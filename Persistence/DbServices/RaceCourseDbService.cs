using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class RaceCourseDbService : BaseDbService<RaceCourse>
{
    public RaceCourseDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceCourse ORDER BY name";
        InsertCommand = "INSERT INTO RaceCourse (Id, Name)  VALUES (@Id, @Name)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var dataList = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in dataList) result[item.Name] = item.Id;
        return result;
    }
}
