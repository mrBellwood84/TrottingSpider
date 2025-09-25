using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class RaceDbService : BaseDbService<Race>
{
    public RaceDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM Race";
        InsertCommand = "INSERT INTO Race (Id, CompetitionId, RaceNumber, Distance, HasGambling)";
    }
}