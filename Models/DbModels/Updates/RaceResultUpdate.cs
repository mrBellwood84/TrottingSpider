namespace Models.DbModels.Updates;

public class RaceResultUpdate
{
    public string Id { get; set; }
    public string Time { get; init; }
    public bool FromDirectSource { get; init; } = true;
}