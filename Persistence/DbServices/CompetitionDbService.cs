using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class CompetitionDbService : BaseDbService<Competition>
{
    public CompetitionDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM Competition";
        InsertCommand = "INSERT INTO Competition (id, raceCourseId, date) " +
                        "VALUES (@Id, @RaceCourseId, @Date)";
    }
}