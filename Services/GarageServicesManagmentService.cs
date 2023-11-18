using GarageApp.Data;
using GarageApp.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
