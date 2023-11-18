﻿using GarageApp.Data;
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

            if (_context.Garages == null)
            {
                throw new Exception("Entity set 'MvcMovieContext.Garages'  is null.");
            }

            if (_context.Specialization == null)
            {
                throw new Exception("Entity set 'MvcMovieContext.Garages'  is null.");
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

            var garageSpecializationsView = new GarageSpecializationsViewModel
            {
                Specializations = new SelectList(await specializations.Distinct().ToListAsync()),
                Garages = await garages.ToListAsync()
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

        public async void DeleteGarage(int id)
        {

            if (_context.Garages == null)
            {
                throw new Exception($"Entity set 'ApplicationDbContext.Garages'  is null.");
            }
            Garage garage = await this.GetGarage(id); ;
            if (garage != null)
            {
                _context.Garages.Remove(garage);
            }

            await _context.SaveChangesAsync();
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
                        var spec = _context.Specialization.First(elem => elem.Id == specializationId);
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

        public Garage CreateGarage(Garage garage, string[] GarageSpecializations)
        {
            //if (_context.Garages.Any(g => g.Name == garage.Name))
            //{
            //    return View(new ErrorViewModel { RequestId = "Garage with same name exists" });
            //}
            //garage.OwnerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            //addSpecializationToGarage(garage, GarageSpecializations);

            //if (ModelState.IsValid)
            //{
            //    _context.Add(garage);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewBag.Specialization = _context.Specialization.ToList();

            return garage;
        }

    }
}
