namespace Models.DbModels;

public class RaceResult
{
    public string Id { get; init; }
    public string RaceStartNumberId { get; init;}
    public int Place { get; init; }
    public int Odds { get; init; }
    public string Time { get; init; }
    public string KmTime  { get; init; }
    public string RRemark { get; init; }
    public string GRemark { get; init; }
    public bool FromDirectSource { get; init; }
}