using Models.Record;

namespace Persistence;

public class CompetitionDbService : BaseDbService<Competition>
{
    public CompetitionDbService(string connectionString) : base(connectionString)
    {
        Query = "SELECT * FROM Competition";
        InsertCommand = "INSERT INTO Competition (id, raceCourseId, date) " +
                        "VALUES (@Id, @RaceCourseId, @Date)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist)
        {
            var key = $"{item.RaceCourseId}_{item.Date}";
            result[key] = item.Id;
        }
        return result;
    }
}