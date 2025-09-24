using Models.Record;

namespace Persistence;

public class HorseDbService : BaseDbService<Horse>
{
    public HorseDbService(string connectionString) : base(connectionString)
    {
        Query = "SELECT * FROM horse ORDER BY SourceId";
        InsertCommand = "INSERT INTO horse (id, sourceId, name, sex, yearOfBirth) " +
                        "VALUES (@Id, @SourceId, @Name, @Sex, @YearOfBirth)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var dataList = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in dataList) result[item.SourceId] = item.Id;
        return result;
    }
}