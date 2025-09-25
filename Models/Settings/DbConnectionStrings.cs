using System.ComponentModel.DataAnnotations;

namespace Models.Settings;

public class DbConnectionStrings
{
    [Required]
    public string Default { get; init; }
}