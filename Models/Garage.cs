using Microsoft.Build.Framework;

namespace GarageApp.Models;

public class Garage
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public Guid OwnerId { get; set; }
}