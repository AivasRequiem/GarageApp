using GarageApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Garage> Garages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
