using Models.Record;

namespace Persistence;

public class DriverDbService : BaseDbService<Driver>
{
    public DriverDbService(string connectionString) : base(connectionString)
    {
        Query = "SELECT * FROM driver ORDER BY SourceId ASC";
        InsertCommand = 
            "INSERT INTO driver (id, driverLicenseId, sourceId, name, yearOfBirth) " +
            "VALUES (@id, @driverLicenseId, @sourceId, @name, @yearOfBirth)";


    }
    
    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist) result[item.SourceId] = item.Id;
        return result;
    }
}