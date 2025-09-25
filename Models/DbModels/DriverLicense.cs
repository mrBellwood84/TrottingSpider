using System.ComponentModel.DataAnnotations;

namespace Models.DbModels;

public class DriverLicense
{
    [Required]
    public string Id { get; init; }
    [Required]
    public string Code { get; init; }
    public string Description { get; init; }
}