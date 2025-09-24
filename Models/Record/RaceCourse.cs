namespace Models.Record;

public class RaceCourse
{
    public string Id { get; set; }
    public string CountryId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}