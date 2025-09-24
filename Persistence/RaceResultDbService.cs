using Models.Record;

namespace Persistence;

public class RaceResultDbService : BaseDbService<RaceResult>
{
    public RaceResultDbService(string connectionString) : base(connectionString)
    {
        Query = "SELECT * FROM RaceResult";
        InsertCommand =
            "INSERT INTO RaceResult (Id, RaceStartNumberId, Place, Time, KmTime, RRemark, GRemark, FromDirectSource)" +
            "VALUES (@Id, @RaceStartNumberId, @Place, @Time, @KmTime, @RRemark, @GRemark, @FromDirectSource)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await base.GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist) result[item.RaceStartNumberId] = item.Id;
        return result;
    }
}