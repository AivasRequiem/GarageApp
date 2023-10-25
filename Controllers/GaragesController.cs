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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Garages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Garage garage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(garage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(garage);
        }

        // GET: Garages/Edit/5
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Garages == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages.FindAsync(id);
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Garage garage)
        {
            if (id != garage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarageExists(garage.Id))
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
            return View(garage);
        }

        // GET: Garages/Delete/5
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "garageOwner")]
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
