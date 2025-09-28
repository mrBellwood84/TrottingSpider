using Models.DbModels;
using Models.ScrapeData;

namespace Scraping.Processors;

public class ProcessDriverScrapeData(Dictionary<string, DriverLicense> driverLicenses)
{
    public DriverLicense NewDriverLicense { get; private set; } = null;
    
    public Driver Process(DriverScrapeData rawData) {
    {
        return new Driver
        {
            Id = Guid.NewGuid().ToString(),
            DriverLicenseId = _parseLicenseCode(rawData.DriverLicense),
            SourceId = rawData.SourceId,
            Name = rawData.Name,
            YearOfBirth = _parseYearOfBirth(rawData.YearOfBirth),
        };
    }}
    
    private int _parseYearOfBirth(string yearOfBirth)
    {
        try
        {
            return int.Parse(yearOfBirth);
        }
        catch (FormatException)
        {
            return 1900;
        }
    }
    private string _parseLicenseCode(string licenseCode)
    {
        var splitted =  licenseCode.Split(')');
        if (splitted.Length < 2)
            return " - ";
        
        var code = splitted[0].Split("(")[1].Trim().ToUpper();
        if (driverLicenses.TryGetValue(code, out var entity)) return entity.Id;

        NewDriverLicense = new DriverLicense
        {
            Id = Guid.NewGuid().ToString(),
            Code = code,
            Description = splitted[1].Trim(),
        };

        return NewDriverLicense.Id;
    }
}