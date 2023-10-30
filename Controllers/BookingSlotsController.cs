using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageApp.Data;
using GarageApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GarageApp.Controllers
{
    public class BookingSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookingSlots
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            Guid userGuid = new Guid(userId);

            var mySlots = _context.BookingSlot
                .Include(slot => slot.GarageService)
                .Include(slot => slot.GarageService.Garage)
                .Where(slot => slot.UserId == userGuid ||
                       slot.GarageService.Garage.OwnerId == userGuid)
                .OrderBy(slot => slot.Date);


            return View(await mySlots.ToListAsync());
        }

        // GET: BookingSlots/Create
        public IActionResult Create(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["GarageServiceId"] = id;
            return View();
        }

        // POST: BookingSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(string? id, [Bind("Date,Description")] BookingSlot bookingSlot)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return NotFound();
                }
                Guid serviceId = new Guid(id);
                var newSlot = new BookingSlot()
                {
                    UserId = new Guid(userId),
                    IsConfirmed = false,
                    Description = bookingSlot.Description,
                    Date = bookingSlot.Date,
                    GarageServiceId = serviceId,
                    Id = Guid.NewGuid()
                };

                _context.Add(newSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(bookingSlot);
        }

        public async Task<IActionResult> Confirm(Guid? id)
        {
            if (id == null || _context.BookingSlot == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            Guid userGuid = new Guid(userId);

            var slot = await _context.BookingSlot
                .Include(s => s.GarageService)
                .Include(s => s.GarageService!.Garage)
                .FirstAsync(s => s.Id == id);
           
            if (slot.GarageService!.Garage.OwnerId != userGuid)
            {
                return NotFound();
            }
            slot.IsConfirmed = true;
            _context.Update(slot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (_context.BookingSlot == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BookingSlot'  is null.");
            }
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            Guid userGuid = new Guid(userId);

            var slot = await _context.BookingSlot
                .Include(s => s.GarageService)
                .Include(s => s.GarageService!.Garage)
                .FirstAsync(s => s.Id == id);

            if (slot.GarageService!.Garage.OwnerId != userGuid && slot.UserId != userGuid)
            {
                return NotFound();
            }

            if (slot != null)
            {
                _context.BookingSlot.Remove(slot);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingSlotExists(Guid id)
        {
          return (_context.BookingSlot?.Any(e => e.GarageServiceId == id)).GetValueOrDefault();
        }
    }
}
