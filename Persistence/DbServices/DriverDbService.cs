using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class DriverDbService : BaseDbService<Driver>
{
    public DriverDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM driver ORDER BY SourceId ASC";
        InsertCommand = 
            "INSERT INTO driver (id, driverLicenseId, sourceId, name, yearOfBirth) " +
            "VALUES (@id, @driverLicenseId, @sourceId, @name, @yearOfBirth)";
    }
}