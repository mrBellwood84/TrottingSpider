using Models.DbModels;
using Models.Settings;

namespace Persistence.Services;

public class RaceCourseDbService : BaseDbService<Racecourse>
{
    public RaceCourseDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceCourse ORDER BY name";
        InsertCommand = "INSERT INTO RaceCourse (Id, Name)  VALUES (@Id, @Name)";
    }
}
