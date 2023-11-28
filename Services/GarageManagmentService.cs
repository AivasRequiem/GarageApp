using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Services
{
    public class GarageManagmentService
    {
        private readonly ApplicationDbContext _context;

        public GarageManagmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GarageSpecializationsViewModel> GetGaragesBySpecialization(string garageSpecialization, string searchString)
        {

            if (_context.Garages == null || _context.Specialization == null)
            {
                GarageSpecializationsViewModel eror = new GarageSpecializationsViewModel
                {
                    isValid = false,
                    Exeption = "Entity set 'MvcMovieContext.Garages'  is null."
                };

                return eror;
            }

            IQueryable<string> specializations = from spec in _context.Specialization
                                                 select spec.Name;

            IQueryable<Garage> garages = from garage in _context.Garages
                                         select garage;

            if (!string.IsNullOrEmpty(searchString))
            {
                garages = garages.Where(garage => garage.Name!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(garageSpecialization))
            {
                garages = garages.Where(g => g.GarageSpecializations != null && g.GarageSpecializations.Any(spec => spec.Specialization.Name == garageSpecialization));
            }

            GarageSpecializationsViewModel garageSpecializationsView = new GarageSpecializationsViewModel
            {
                Specializations = new SelectList(await specializations.Distinct().ToListAsync()),
                Garages = await garages.ToListAsync(),
                isValid = true
            };

            return garageSpecializationsView;
        }

        public async Task<Garage> GetGarage(int? id)
        {
            return await _context.Garages
                .Include(g => g.GarageSpecializations)
                .ThenInclude(gs => gs.Specialization)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Garage>> GetAllGarages()
        {
            return await _context.Garages.ToListAsync();
        }

        public Boolean IsExistGarage(Garage garage)
        {
            return _context.Garages.Any(g => g.Name == garage.Name);
        }

        public async Task<bool> DeleteGarage(int id)
        {

            if (_context.Garages == null)
            {
                return false;
            }
            
            _context.Garages.Remove(await GetGarage(id));

            await _context.SaveChangesAsync();

            return true;
        }

        private void addSpecializationToGarage(Garage garage, string[] GarageSpecializations)
        {
            if (GarageSpecializations != null)
            {
                foreach (var specialization in GarageSpecializations)
                {
                    Guid specializationId;
                    if (Guid.TryParse(specialization, out specializationId))
                    {
                        Specialization spec = _context.Specialization.First(elem => elem.Id == specializationId);
                        garage.GarageSpecializations.Add(new GarageSpecializations()
                        {
                            Garage = garage,
                            Specialization = spec,
                            SpecializationId = specializationId
                        });
                    }
                }
            }
        }

        public async Task<Boolean> CreateGarage(Garage garage, string[] GarageSpecializations, Guid OwnerId)
        {
            try
            {           
                addSpecializationToGarage(garage, GarageSpecializations);
                garage.OwnerId = OwnerId;
                _context.Add(garage);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Boolean> EditGarage(Garage existingGarage, Garage garage, string[] GarageSpecializations)
        {
            try
            {
                if (GarageSpecializations != null)
                {

                    existingGarage.Name = garage.Name;
                    existingGarage.OwnerId = garage.OwnerId;
                    existingGarage.Description = garage.Description;
                    existingGarage.AwailablePlaces = garage.AwailablePlaces;

                    if (existingGarage.GarageSpecializations != null)
                    {
                        existingGarage.GarageSpecializations.Clear();
                    }
                    else
                    {
                        existingGarage.GarageSpecializations = new List<GarageSpecializations>();
                    }

                    addSpecializationToGarage(existingGarage, GarageSpecializations);
                }
                try
                {
                    _context.Update(existingGarage);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public Boolean IsAnyGarages()
        {
            return _context.Garages == null;
        }
    }    
}
