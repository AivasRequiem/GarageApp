using GarageApp.Data;
using GarageApp.Models;
using Microsoft.EntityFrameworkCore;
using static GarageApp.Controllers.GarageServicesController;

namespace GarageApp.Services
{
    public class GarageServicesManagmentService
    {
        private readonly ApplicationDbContext _context;

        public GarageServicesManagmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GarageService>> GetGarageServicesByGarageId(int? id)
        {
            return await _context.GarageService.Where(elem => elem.GarageId == id).ToListAsync();
        }
    
        public async Task<GarageService> GetGarageService(Guid? Id)
        {
            return await _context.GarageService
                .Include(g => g.Garage)
                .FirstOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<IEnumerable<GarageService>> GetAllGarageServices()
        {
            return await _context.GarageService.Include(g => g.Garage).ToListAsync();
        }

        public async Task CreateService(int id, GarageServiceFromForm garageService)
        {
            GarageService newService = new GarageService()
            {
                Description = garageService.Description,
                Name = garageService.Name,
                Price = garageService.Price,
                Garage = await _context.Garages.FirstAsync(elem => elem.Id == id),
                GarageId = id,
                SpecializationId = garageService.SpecializationId,
                Specialization = await _context.Specialization.FirstAsync(elem => elem.Id == garageService.SpecializationId),
            };
            _context.Add(newService);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteService(Guid id)
        {
            if (IsAnyGarageServices())
            {
                return false;
            }

            GarageService garageService = await GetGarageService(id);

            if (garageService != null)
            {
                _context.GarageService.Remove(garageService);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task EditService(GarageService garageService)
        {
            GarageService existingService = await GetGarageService(garageService.Id);
            existingService.Price = garageService.Price;
            existingService.Description = garageService.Description;
            existingService.Name = garageService.Name;
            _context.Update(existingService);
            await _context.SaveChangesAsync();
        }

        public Boolean IsAnyGarageServices()
        {
            return _context.GarageService == null;
        }
    }
}
