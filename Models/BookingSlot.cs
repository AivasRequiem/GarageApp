using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageApp.Models
{
    public class BookingSlot
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("GarageService")]
        public Guid GarageServiceId { get; set; }

        public GarageService? GarageService { get; set; }

        public Guid? UserId { get; set; }

        public string? Description { get; set; }

        public bool IsConfirmed { get; set; }
	}
}