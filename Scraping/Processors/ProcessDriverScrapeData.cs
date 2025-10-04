using Models.DbModels;
using Models.ScrapeData;

namespace Scraping.Processors;

public class ProcessDriverScrapeData
{
    public DriverLicense NewDriverLicense { get; private set; }
    
    public Driver Process(DriverScrapeData rawData) {
    {
        NewDriverLicense = _parseLicenseCode(rawData.DriverLicense);
            
        return new Driver
        {
            Id = Guid.NewGuid().ToString(),
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
    private DriverLicense _parseLicenseCode(string licenseCode)
    {
        try
        {
            var splitted = licenseCode.Split(')');
            var code = splitted[0].Split("(")[1].Trim().ToUpper();

            NewDriverLicense = new DriverLicense
            {
                Id = Guid.NewGuid().ToString(),
                Code = code,
                Description = splitted[1].Trim(),
            };

            return NewDriverLicense;
        }
        catch (IndexOutOfRangeException) {}
        
        return new DriverLicense
        {
            Id = Guid.NewGuid().ToString(),
            Code = "-",
            Description = "Ikke gyldig lisens",
        };

    }
}