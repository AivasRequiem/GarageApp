using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarageApp.Models
{
    public class GarageSpecializationsViewModel
    {
        public List<Garage>? Garages { get; set; }
        public SelectList? Specializations { get; set; }
        public string? GarageSpecialization { get; set; }
        public string? SearchString { get; set; }
    }
}

