using GarageApp.Data;
using GarageApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Services
{
    public class SpecializationManagmentService
    {
        private readonly ApplicationDbContext _context;

        public SpecializationManagmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Specialization>> GetAllSpecializations()
        {
            return await _context.Specialization.ToListAsync();
        }

        public async Task<Specialization> GetSpecializationById(Guid id)
        {
            return await _context.Specialization.FirstOrDefaultAsync(elem => elem.Id == id);
        }
    }
}
