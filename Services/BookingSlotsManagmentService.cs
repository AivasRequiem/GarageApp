using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GarageApp.Services
{
    public class BookingSlotsManagmentService
    {
        private readonly ApplicationDbContext _context;

        public BookingSlotsManagmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingSlot>> GetBookingSlotsOfUser(Guid userId) { 
            return await _context.BookingSlot
                .Include(slot => slot.GarageService)
                .Include(slot => slot.GarageService.Garage)
                .Where(slot => slot.UserId == userId ||
                       slot.GarageService.Garage.OwnerId == userId)
                .OrderBy(slot => slot.Date).ToListAsync();
        }

        public async Task<BookingSlot> GetBookingSlotById(Guid id)
        {
            return await _context.BookingSlot.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task EditBookingSlot(Guid bookingId, Guid userId, BookingSlot bookingSlot)
        {
            if (bookingId != bookingSlot.Id)
            {
                throw new Exception($"Booking Slot with ID {bookingId} doesn't match.");
            }

            if (userId == bookingSlot.UserId)
            {
                throw new Exception("You have no rights");
            }

            BookingSlot editedBookingSlot = await this.GetBookingSlotById(bookingId);

            if (editedBookingSlot != null)
            {
                editedBookingSlot.Date = bookingSlot.Date;
                editedBookingSlot.Description = bookingSlot.Description;
            }

            _context.Update(editedBookingSlot);
            await _context.SaveChangesAsync();
        }

        public async Task CreateBookingSlot(BookingSlot bookingSlot, Guid userId, Guid serviceId)
        {
            if (bookingSlot.Date < DateTime.Now)
            {
                throw new Exception();
            }

            IdentityUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());
            string? phoneNumber = user.PhoneNumber;
            string? userEmail = user.Email;
            bookingSlot.Description += "\n" + userEmail + " " + phoneNumber;

            BookingSlot newSlot = new BookingSlot()
            {
                UserId = userId,
                IsConfirmed = false,
                Description = bookingSlot.Description,
                Date = bookingSlot.Date,
                GarageServiceId = serviceId,
                Id = Guid.NewGuid()
            };

            _context.Add(newSlot);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmBookingSlot(Guid? bookingId, Guid userId)
        {
            if (bookingId == null || _context.BookingSlot == null)
            {
                throw new Exception();
            }

            BookingSlot slot = await _context.BookingSlot
                .Include(s => s.GarageService)
                .Include(s => s.GarageService!.Garage)
                .FirstAsync(s => s.Id == bookingId);

            if (slot.GarageService!.Garage.OwnerId != userId)
            {
                throw new Exception();
            }

            slot.IsConfirmed = true;
            _context.Update(slot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingSlot(Guid? bookingId, Guid userId)
        {
            if (_context.BookingSlot == null)
            {
                throw new Exception();
            }

            BookingSlot slot = await _context.BookingSlot
                .Include(s => s.GarageService)
                .Include(s => s.GarageService!.Garage)
                .FirstAsync(s => s.Id == bookingId);

            if (slot.GarageService!.Garage.OwnerId != userId && slot.UserId != userId)
            {
                throw new Exception();
            }

            if (slot != null)
            {
                _context.BookingSlot.Remove(slot);
            }

            await _context.SaveChangesAsync();
        }
    }

    
}
