using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageApp.Models
{
    public class GarageServise
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        [ForeignKey("Garage")]
        public int GarageId { get; set; }
        public Garage Garage { get; set; }
        [ForeignKey("Specialization")]
        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
    }
}
