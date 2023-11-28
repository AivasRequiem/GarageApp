using GarageApp.Data;
using GarageApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace GarageApp.Services
{
    public class SpecializationManagmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly GarageManagmentService _garageManagment;

        public SpecializationManagmentService(ApplicationDbContext context)
        {
            _context = context;
            _garageManagment = new GarageManagmentService(context);
        }
        public async Task<IEnumerable<Specialization>> GetAllSpecializations()
        {
            return await _context.Specialization.ToListAsync();
        }

        public async Task<Specialization> GetSpecializationById(Guid? id)
        {
            return await _context.Specialization.FirstOrDefaultAsync(elem => elem.Id == id);
        }

        public async Task<List<Specialization>> GetSpecializationOfGarage(int id)
        {
            Garage garage = await _garageManagment.GetGarage(id);
            List<Specialization> specializations = garage.GarageSpecializations
                .Select(gs => gs.Specialization)
                .ToList();

            return specializations;
        }
        public bool CheckContext()
        {
            return _context.Specialization != null;
        }
        public async Task CreateSpecialization(Specialization specialization)
        {
            specialization.Id = Guid.NewGuid();
            _context.Add(specialization);
            await _context.SaveChangesAsync();
        }
        public async Task EditSpecialization(Specialization specialization)
        {
            _context.Update(specialization);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteSpecialization(Guid Id)
        {
            if (_context.Specialization == null)
            {
                return false;
            }
            Specialization specialization = await this.GetSpecializationById(Id);
            if (specialization != null)
            {
                _context.Specialization.Remove(specialization);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
