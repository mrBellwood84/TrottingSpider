namespace Models.ScrapeData;

/** Driver Scrape data example:
    SourceId: 4265
    Name: Jan Lyng
    YearOfBirth: 1970
    DriverLicense: ( D1 ) Amatør, trene og kjøre
*/

public struct DriverScrapeData
{
    public string SourceId {get;init;}
    public string Name { get; init; }
    public string YearOfBirth { get; init; }
    public string DriverLicense { get; init; }
}