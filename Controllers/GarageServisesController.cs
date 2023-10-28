using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageApp.Data;
using GarageApp.Models;
using System.ComponentModel.DataAnnotations;

namespace GarageApp.Controllers
{
    public class GarageServisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GarageServisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GarageServises
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GarageServise.Include(g => g.Garage);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GarageServises/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.GarageServise == null)
            {
                return NotFound();
            }

            var garageServise = await _context.GarageServise
                .Include(g => g.Garage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (garageServise == null)
            {
                return NotFound();
            }

            return View(garageServise);
        }

        // GET: GarageServises/Create/garageId
        public IActionResult Create(int garageId)
        {
            ViewData["GarageId"] = garageId;
            return View();
        }

        public class GarageServiseFromForm
        {
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; } //change to Description
            public decimal? Price { get; set; }
            //add specialization
        }

        // POST: GarageServises/Create/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Name,Description,Price")] GarageServiseFromForm garageServise)
        {            
            if (ModelState.IsValid)
            {
                GarageServise newServise = new GarageServise()
                {
                    Description = garageServise.Description,
                    Name = garageServise.Name,
                    Price = garageServise.Price,
                    Garage = await _context.Garages.FirstAsync(elem => elem.Id == id),
                    GarageId = id
                };
                _context.Add(newServise);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Garages", new { id = id });
            }

            return View(garageServise);
        }

        // GET: GarageServises/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.GarageServise == null)
            {
                return NotFound();
            }

            var garageServise = await _context.GarageServise.FindAsync(id);
            if (garageServise == null)
            {
                return NotFound();
            }
            ViewData["GarageId"] = new SelectList(_context.Garages, "Id", "Id", garageServise.GarageId);
            return View(garageServise);
        }

        // POST: GarageServises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Price,GarageId")] GarageServise garageServise)
        {
            if (id != garageServise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garageServise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarageServiseExists(garageServise.Id))
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
            ViewData["GarageId"] = new SelectList(_context.Garages, "Id", "Id", garageServise.GarageId);
            return View(garageServise);
        }

        // GET: GarageServises/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.GarageServise == null)
            {
                return NotFound();
            }

            var garageServise = await _context.GarageServise
                .Include(g => g.Garage)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (garageServise == null)
            {
                return NotFound();
            }

            return View(garageServise);
        }

        // POST: GarageServises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.GarageServise == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GarageServise'  is null.");
            }
            var garageServise = await _context.GarageServise.FindAsync(id);
            if (garageServise != null)
            {
                _context.GarageServise.Remove(garageServise);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GarageServiseExists(Guid id)
        {
          return (_context.GarageServise?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
