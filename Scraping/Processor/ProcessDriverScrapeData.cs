using Application.AppLogger;
using Models.Record;
using Models.ScrapeData;
using Persistence;
using Scraping.Spider.NO;

namespace Scraping.Processor;

public class ProcessDriverScrapeData(Dictionary<string, string> licenseCodeDict)
{
    private Dictionary<string, string> LicenseCodeDict = licenseCodeDict;
    private readonly AppLogger _logger = new AppLogger();

    public DriverLicense? CreateNewDriverLicense { get; set; } = null;

    public Driver Process(DriverScapeData data)
    {
        var driver = new Driver
        {
            Id = Guid.NewGuid().ToString(),
            SourceId = data.SourceId,
            Name = data.Name,
            YearOfBirth = _parseYearOfBirth(data.YearOfBirth, data.SourceId),
            DriverLicenseId = _parseLicenseCode(data.DriverLicense, data.SourceId)
        };
        return driver;
    }

    private int _parseYearOfBirth(string yearOfBirth, string sourceId)
    {
        try
        {
            return int.Parse(yearOfBirth);
        }
        catch (FormatException)
        {
            _logger.LogError($"ProcessDriverScrapeData: Invalid year of birth '{yearOfBirth}', driver: {sourceId}");
            _logger.LogInformation($"ProcessDriverScrapeData: Year of birth set to 1900 for driver: '{sourceId}'");
            return 1900;
        }
    }

    private string _parseLicenseCode(string licenseCode, string sourceId)
    {
        var splitted =  licenseCode.Split(')');
        if (splitted.Length < 2)
        {
            _logger.LogError($"ProcessDriverScrapeData: Invalid licence field provided: '{licenseCode}' for driver: {sourceId}");
            _logger.LogError($"ProcessDriverScrapeData: Code set to ' - ' for driver: '{sourceId}'");
            return " - ";
        }
        var code = splitted[0].Split("(")[1].Trim().ToUpper();
        if (LicenseCodeDict.TryGetValue(code, out var id)) return id;
        
        _logger.LogWarning($"ProcessDriverScrapeData: Driver Licence Code not found: '{code}'");
        _logger.LogWarning($"New DriverLicense will be registered");

        var newDriverLicence = new DriverLicense
        {
            Id = Guid.NewGuid().ToString(),
            Code = code,
            Description = splitted[1].Trim(),
        };
        
        CreateNewDriverLicense = newDriverLicence;
        return newDriverLicence.Id;
    }
}