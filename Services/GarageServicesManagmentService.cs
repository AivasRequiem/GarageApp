using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task DeleteService(Guid id)
        {
            if (IsAnyGarageServices())
            {
                throw new Exception("Entity set 'ApplicationDbContext.GarageService'  is null.");
            }
            var garageService = await GetGarageService(id);
            if (garageService != null)
            {
                _context.GarageService.Remove(garageService);
            }

            await _context.SaveChangesAsync();
        }

        public async Task EditService(GarageService garageService)
        {
            _context.Update(garageService);
            await _context.SaveChangesAsync();
        }

        public Boolean IsAnyGarageServices()
        {
            return _context.GarageService == null;
        }
    }
}
