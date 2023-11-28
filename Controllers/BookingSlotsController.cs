using Microsoft.AspNetCore.Mvc;
using GarageApp.Data;
using GarageApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GarageApp.Services;
using FluentValidation.Results;

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

                ValidationResult valResult = await _bookingSlotsManagmentService.CreateBookingSlot(bookingSlot, userId, new Guid(id));
                if (valResult.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["GarageServiceId"] = id;
                    DisplayValidationErrors(valResult);
                    return View(bookingSlot);
                }            
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Date,Description")] BookingSlot bookingSlot)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bookingSlot.Id = id;

            if (ModelState.IsValid)
            {
                ValidationResult valResult =  await _bookingSlotsManagmentService.EditBookingSlot(userId, bookingSlot);

                if (!valResult.IsValid)
                {
                    ViewData["GarageServiceId"] = id.ToString();
                    DisplayValidationErrors(valResult);
                    return View(bookingSlot);
                }
                
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: BookingSlots/Confirm
        public async Task<IActionResult> Confirm(Guid? id)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ValidationResult valResult = await _bookingSlotsManagmentService.ConfirmBookingSlot(id, userId);
            if (!valResult.IsValid)
            {
                TempData["Error"] = valResult.Errors[0].ErrorMessage;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: BookingSlots/Delete
        public async Task<IActionResult> Delete(Guid? id)
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ValidationResult valResult = await _bookingSlotsManagmentService.DeleteBookingSlot(id, userId);
            if (!valResult.IsValid)
            {
                TempData["Error"] = valResult.Errors[0].ErrorMessage;
            }
            return RedirectToAction(nameof(Index));
        }

        private void DisplayValidationErrors(ValidationResult valResult)
        {
            foreach (var error in valResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
