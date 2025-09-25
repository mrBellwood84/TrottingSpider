using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class DriverLicenseDbService : BaseDbService<DriverLicense>
{
    public DriverLicenseDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {  
        Query = "SELECT * FROM DriverLicense ORDER BY code";
        InsertCommand = "INSERT INTO DriverLicense (id, code, description) " +
                        "VALUES (@Id, @Code, @Description)";
    }
}