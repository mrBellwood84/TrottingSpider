using System.ComponentModel.DataAnnotations;

namespace Models.DbModels;

public class Competition
{
    [Required]
    public string Id { get; init; }
    [Required]
    public string RaceCourseId { get; init; }
    [Required]
    public string Date { get; init; }
    [Required]
    public bool StartlistFromSource { get; init; } = false;
    [Required] 
    public bool ResultsFromSource { get; init; } = false;
}