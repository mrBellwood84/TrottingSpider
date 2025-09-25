using System.ComponentModel.DataAnnotations;

namespace Models.DbModels;

public class RaceCourse
{
    [Required]
    public string Id { get; init; }
    public string CountryId { get; init; }
    [Required]
    public string Name { get; init; }
    public string Code { get; init; }
    public float Latitude { get; init; }
    public float Longitude { get; init; }
}