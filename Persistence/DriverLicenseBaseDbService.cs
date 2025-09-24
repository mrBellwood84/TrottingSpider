using Dapper;
using Models.Record;

namespace Persistence;

public class DriverLicenseBaseDbService : BaseDbService<DriverLicense>
{
    public DriverLicenseBaseDbService(string connectionString) : base(connectionString)
    {  
        Query = "SELECT * FROM DriverLicense ORDER BY code";
        InsertCommand = "INSERT INTO DriverLicense (id, code, description) " +
                        "VALUES (@Id, @Code, @Description)";
    }
    
    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist)  result[item.Code] = item.Id;
        return result;
    }
}