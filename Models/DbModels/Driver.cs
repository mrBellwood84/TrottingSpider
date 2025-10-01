namespace Models.DbModels;

public class Driver
{
    public string Id { get; init; }
    public string DriverLicenseId { get; set; }
    public string SourceId { get; init; }
    public string Name { get; init; }
    public int YearOfBirth { get; init; }
}