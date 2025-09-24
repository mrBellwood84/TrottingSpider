namespace Models.Record;

public class RaceResult
{
    public string Id { get; set; }
    public string RaceStartNumberId { get; set;}
    public int Place { get; set; }
    public string Time { get; set; }
    public string KmTime  { get; set; }
    public string RRemark { get; set; }
    public string GRemark { get; set; }
    public bool FromDirectSource { get; set; }
}