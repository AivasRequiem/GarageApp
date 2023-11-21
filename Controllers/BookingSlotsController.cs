using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarageApp.Data;
using GarageApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GarageApp.Services;

namespace GarageApp.Controllers
{
    public class BookingSlotsController : Controller
    {
        private readonly BookingSlotsManagmentService _bookingSlotsManagmentService;

        public BookingSlotsController(ApplicationDbContext context)
        {
            _bookingSlotsManagmentService = new BookingSlotsManagmentService(context);
        }

        // GET: BookingSlots
        public async Task<IActionResult> Index()
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<BookingSlot> mySlots = await _bookingSlotsManagmentService.GetBookingSlotsOfUser(userId);

            return View(mySlots);
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
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    await _bookingSlotsManagmentService.CreateBookingSlot(bookingSlot, userId, new Guid(id));
                }
                catch
                {
                    ViewData["GarageServiceId"] = id;
                    return View(bookingSlot);
                }             

                return RedirectToAction(nameof(Index));
            }

            ViewData["GarageServiceId"] = id;
            return View(bookingSlot);
        }

        // GET: BookingSlots/Edit
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BookingSlot bookingSlot = await _bookingSlotsManagmentService.GetBookingSlotById(new Guid(id));
            ViewData["GarageServiceId"] = id;
            return View(bookingSlot);
        }

        // POST: BookingSlots/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,Description")] BookingSlot bookingSlot)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingSlotsManagmentService.EditBookingSlot(id, userId, bookingSlot);
                }
                catch (Exception ex)
                {
                    return View(new ErrorViewModel { RequestId = ex.Message });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookingSlot);
        }

        // POST: BookingSlots/Confirm
        public async Task<IActionResult> Confirm(Guid? id)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _bookingSlotsManagmentService.ConfirmBookingSlot(id, userId);
            }
            catch 
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: BookingSlots/Delete
        public async Task<IActionResult> Delete(Guid? id)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _bookingSlotsManagmentService.DeleteBookingSlot(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
