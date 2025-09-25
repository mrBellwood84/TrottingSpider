using System.ComponentModel.DataAnnotations;

namespace Models.Settings;

public class SpiderUrlCollection
{
    [Required]
    public string NoCalendarUrl { get; init; }
    [Required]
    public string NoBaseDriverUrl { get; init; }
    [Required]
    public string NoBaseHorseUrl { get; init; }
}