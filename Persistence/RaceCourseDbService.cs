using Models.Record;

namespace Persistence;

public class RaceCourseDbService : BaseDbService<RaceCourse>
{
    public RaceCourseDbService(string connectionString) : base(connectionString)
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
