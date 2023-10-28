using System.Data;

namespace GarageApp.Models
{
    public class GarageSpecializations
    {
        public int GarageId { get; set; }
        public Garage Garage { get; set; }

        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
    }
}
