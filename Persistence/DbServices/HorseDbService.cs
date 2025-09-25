using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class HorseDbService : BaseDbService<Horse>
{
    public HorseDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM horse ORDER BY SourceId";
        InsertCommand = "INSERT INTO horse (id, sourceId, name, sex, yearOfBirth) " +
                        "VALUES (@Id, @SourceId, @Name, @Sex, @YearOfBirth)";
    }
}