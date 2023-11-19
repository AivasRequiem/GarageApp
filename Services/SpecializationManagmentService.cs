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

        public async Task<Specialization> GetSpecializationById(Guid id)
        {
            return await _context.Specialization.FirstOrDefaultAsync(elem => elem.Id == id);
        }

        public async Task<List<Specialization>> GetSpecializationOfGarage(int id)
        {
            Garage garage = await _garageManagment.GetGarage(id);
            List<Specialization> specializations = garage.GarageSpecializations
                .Select(gs => gs.Specialization)
                .ToList();

            if (specializations.IsNullOrEmpty())
            {
                throw new Exception("Add specializations to Gatage");
            }
            return specializations;
        }
    }
}
