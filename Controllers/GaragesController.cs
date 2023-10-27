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

namespace GarageApp.Controllers
{
    public class GaragesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GaragesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Garages
        public async Task<IActionResult> Index()
        {
              return _context.Garages != null ? 
                          View(await _context.Garages.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Garages'  is null.");
        }

        [HttpGet("GetGarages")]
        public async Task<IEnumerable<Garage>> GetGarages()
        {
            return await _context.Garages.ToListAsync();
        }

        // GET: Garages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Garages == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        // GET: Garages/Create
        [Authorize(Roles = "Admin,garageOwner")]
        public IActionResult Create()
        {
            ViewBag.Specialization = _context.Specialization.ToList();
            return View();
        }

        // POST: Garages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create(Garage garage, string[] GarageSpecializations)
        {
            if (_context.Garages.Any(g => g.Name == garage.Name))
            {
                return BadRequest();
            }
            garage.OwnerId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (GarageSpecializations != null)
            {
                foreach (var specialization in GarageSpecializations)
                {
                    Guid specializationId;
                    if (Guid.TryParse(specialization, out specializationId))
                    {
                        var spec = _context.Specialization.First(elem => elem.Id == specializationId);
                        garage.GarageSpecializations.Add(new GarageSpecializations() { 
                                Garage = garage, 
                                Specialization = spec, SpecializationId = specializationId });
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(garage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

            ViewBag.Specialization = _context.Specialization.ToList();

            Garage garage = _context.Garages
                .Include(g => g.GarageSpecializations)
                .ThenInclude(gs => gs.Specialization)
                .FirstOrDefault(g => g.Id == id);

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
                return NotFound();
            }

            var existingGarage = _context.Garages
                        .Include(g => g.GarageSpecializations)
                        .FirstOrDefault(g => g.Id == id);

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
                    // move to funk same in creation            
                    foreach (var specialization in GarageSpecializations)
                    {
                        Guid specializationId;
                        if (Guid.TryParse(specialization, out specializationId))
                        {
                            var spec = _context.Specialization.First(elem => elem.Id == specializationId);
                            existingGarage.GarageSpecializations.Add(new GarageSpecializations()
                            {
                                Garage = existingGarage,
                                Specialization = spec,
                                SpecializationId = specializationId
                            });
                        }
                    }
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

            var garage = await _context.Garages
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Garages == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Garages'  is null.");
            }
            var garage = await _context.Garages.FindAsync(id);
            if (garage != null)
            {
                _context.Garages.Remove(garage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GarageExists(int id)
        {
          return (_context.Garages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
