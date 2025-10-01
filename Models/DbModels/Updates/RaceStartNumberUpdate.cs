namespace Models.DbModels.Updates;

public class RaceStartNumberUpdate
{
    public string Id { get; set; }
    public string Turn { get; init; }
    public string Auto { get; init; }
    public bool HasGambling  { get; init; }
    public bool FromDirectSource { get; init; } = true;
}