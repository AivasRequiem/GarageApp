using System.ComponentModel.DataAnnotations;

namespace GarageApp.Models
{
    public class Specialization
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
