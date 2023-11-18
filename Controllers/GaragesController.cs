using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using GarageApp.Services;

namespace GarageApp.Controllers
{
    public class GaragesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GarageManagmentService _garageManagmentService;
        private readonly SpecializationManagmentService _specializationManagmentService;
        private readonly GarageServicesManagmentService _garageServicesManagmentService;

        public GaragesController(ApplicationDbContext context)
        {
            _context = context;
            _garageManagmentService = new GarageManagmentService(context);
            _specializationManagmentService = new SpecializationManagmentService(context);
            _garageServicesManagmentService = new GarageServicesManagmentService(context);
        }

        // GET: Garages
        public async Task<IActionResult> Index(string garageSpecialization, string searchString)
        {
            try
            {
                return View(await _garageManagmentService.GetGaragesBySpecialization(garageSpecialization, searchString));
            }
            catch(Exception ex)
            {
                return View(new ErrorViewModel { RequestId = ex.Message });
            }
        }

        [HttpGet("GetGarages")]
        public async Task<IEnumerable<Garage>> GetGarages()
        {
            return await _garageManagmentService.GetAllGarages();
        }

        // GET: Garages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Garages == null)
            {
                return NotFound();
            }

            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }

            ViewBag.GarageServices = await _garageServicesManagmentService.GetGarageServicesByGarageId(id);
            return View(garage);
        }

        // GET: Garages/Create
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();
            return View();
        }

        private async void addSpecializationToGarage(Garage garage, string[] GarageSpecializations)
        {
            if (GarageSpecializations != null)
            {
                foreach (var specialization in GarageSpecializations)
                {
                    Guid specializationId;
                    if (Guid.TryParse(specialization, out specializationId))
                    {
                        Specialization spec = await _specializationManagmentService.GetSpecializationById(specializationId);

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

        // POST: Garages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create(Garage garage, string[] GarageSpecializations)
        {
            if (_garageManagmentService.IsExistGarage(garage))
            {
                return View(new ErrorViewModel { RequestId = "Garage with same name exists" });
            }
            garage.OwnerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            addSpecializationToGarage(garage, GarageSpecializations);

            if (ModelState.IsValid)
            {
                _context.Add(garage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();
            return View(garage);
        }

        // GET: Garages/Edit/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Garages == null)
            {
                return NotFound();
            }

            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }
            return View(garage);
        }

        // POST: Garages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(int id, Garage garage, string[] GarageSpecializations)
        {
            if (id != garage.Id)
            {
                return View(new ErrorViewModel { RequestId = $"Garage with ID {id} doesn't exist" });
            }

            Garage existingGarage = await _garageManagmentService.GetGarage(id); ;

            if (ModelState.IsValid)
            {
                if (GarageSpecializations != null)
                {

                    existingGarage.Name = garage.Name;
                    existingGarage.OwnerId = garage.OwnerId;

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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarageExists(existingGarage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(existingGarage);
        }

        // GET: Garages/Delete/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Garages == null)
            {
                return NotFound();
            }

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        // POST: Garages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _garageManagmentService.DeleteGarage(id);
            }
            catch(Exception ex)
            {
                return View(new ErrorViewModel { RequestId = ex.Message });
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GarageExists(int id)
        {
          return (_context.Garages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
