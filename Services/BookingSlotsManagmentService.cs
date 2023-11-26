using FluentValidation;
using FluentValidation.Results;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GarageApp.Services
{
    public class SlotCreationValidator : AbstractValidator<BookingSlot>
    {
        public SlotCreationValidator(Guid userId)
        {
            RuleFor(slot => slot.Date)
            .Must(date => date > DateTime.Today)
            .WithMessage("The booking date should be at least tommorow");

            RuleFor(slot => slot.Description)
            .Must(desc => desc == null || desc.Length <= 300)
            .WithMessage("Description should be shorter than 300 symbols");

            RuleFor(slot => slot.IsConfirmed)
            .Must(confirmed => !confirmed)
            .WithMessage("Can't edit a confirmed slot");

            RuleFor(slot => slot.UserId)
            .Must(id => id == userId)
            .WithMessage("You are not the creator of the slot");
        }
    }

    public class SlotConfirmValidator : AbstractValidator<BookingSlot>
    {
        public SlotConfirmValidator(Guid ownerId)
        {
            RuleFor(slot => slot.Date)
            .Must(date => date > DateTime.Now.AddHours(1))
            .WithMessage("Can't confirm slot, the scheduled time will be less than in an hour.");

            RuleFor(slot => slot.IsConfirmed)
            .Must(confirmed => !confirmed)
            .WithMessage("The slot is already confirmed");

            RuleFor(slot => slot.GarageService!.Garage.OwnerId)
            .Must(id => ownerId == id)
            .WithMessage("You are forbidden to confirm the slot");
        }
    }
    public class BookingSlotsManagmentService
    {
        private readonly ApplicationDbContext _context;

        public BookingSlotsManagmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingSlot>> GetBookingSlotsOfUser(Guid userId)
        { 
            return await _context.BookingSlot
                .Include(slot => slot.GarageService)
                .Include(slot => slot.GarageService!.Garage)
                .Where(slot => slot.UserId == userId ||
                               slot.GarageService!.Garage.OwnerId == userId)
                .OrderBy(slot => slot.Date).ToListAsync();
        }

        public async Task<BookingSlot> GetBookingSlotById(Guid id)
        {
            return await _context.BookingSlot.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ValidationResult> EditBookingSlot(Guid userId, BookingSlot bookingSlot)
        {
            BookingSlot editedBookingSlot = await GetBookingSlotById(bookingSlot.Id);
            if (editedBookingSlot == null)
            {
                var idValResult = new ValidationResult();
                idValResult.Errors = new List<ValidationFailure>() { new ValidationFailure("ID", "Slot doesn't exist") };
                return idValResult;
            }

            editedBookingSlot.Date = bookingSlot.Date;
            editedBookingSlot.Description = bookingSlot.Description;

            var validator = new SlotCreationValidator(userId);
            var valResult = await validator.ValidateAsync(editedBookingSlot);

            if (valResult.IsValid)
            {
                _context.Update(editedBookingSlot);
                await _context.SaveChangesAsync();
            }
            return valResult;
        }

        public async Task<ValidationResult> CreateBookingSlot(BookingSlot bookingSlot, Guid userId, Guid serviceId)
        {
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

            var validator = new SlotCreationValidator(userId);
            var valResult = await validator.ValidateAsync(newSlot);
            if (valResult.IsValid)
            {
                _context.Add(newSlot);
                await _context.SaveChangesAsync();
            }

            return valResult;
        }

        public async Task<ValidationResult> ConfirmBookingSlot(Guid? bookingId, Guid userId)
        {
            if (bookingId == null || _context.BookingSlot == null)
            {
                var idValResult = new ValidationResult();
                idValResult.Errors = new List<ValidationFailure>() { new ValidationFailure("ID", "Slot doesn't exist") };
                return idValResult;
            }

            BookingSlot slot = await _context.BookingSlot
                .Include(s => s.GarageService)
                .Include(s => s.GarageService!.Garage)
                .FirstAsync(s => s.Id == bookingId);

            var validator = new SlotConfirmValidator(userId);
            var valResult = await validator.ValidateAsync(slot);
            if (valResult.IsValid)
            {
                slot.IsConfirmed = true;
                _context.Update(slot);
                await _context.SaveChangesAsync();
            }

            return valResult;
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
