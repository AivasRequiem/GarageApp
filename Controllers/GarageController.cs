using GarageApp.Database;
using GarageApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GarageController : ControllerBase
    {

        private readonly ILogger<GarageController> _logger;
        private readonly AppDbContext _appDbContext;

        public GarageController(ILogger<GarageController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        [HttpGet("GetGarages")]
        public async Task<IEnumerable<Garage>> GetGarages()
        {
            return await _appDbContext.Garages.ToListAsync();
        }

        [HttpPost("AddGarage")]
        public async Task<IEnumerable<Garage>> CreateGarage(Garage newGarage)
        {
            _appDbContext.Garages.Add(newGarage);
            await _appDbContext.SaveChangesAsync();
            return await _appDbContext.Garages.ToListAsync();
        }
    }
}