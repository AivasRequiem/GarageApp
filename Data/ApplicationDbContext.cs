using GarageApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Garage> Garages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GarageSpecializations>()
                .HasKey(gr => new { gr.GarageId, gr.SpecializationId });

            modelBuilder.Entity<GarageSpecializations>()
                .HasOne(gr => gr.Garage)
                .WithMany(g => g.GarageSpecializations)
                .HasForeignKey(gr => gr.GarageId);

            modelBuilder.Entity<GarageSpecializations>()
                .HasOne(gr => gr.Specialization)
                .WithMany()
                .HasForeignKey(gr => gr.SpecializationId);

        }

        public DbSet<Specialization> Specialization { get; set; } = default!;

        public DbSet<GarageApp.Models.GarageServise> GarageServise { get; set; } = default!;

    }
}