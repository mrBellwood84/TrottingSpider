using Models.DbModels;
using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface IRaceResultsDbServiceExtension
{
    Task InsertBulkAsync(List<RaceResult> data);
    Task UpdateAsync(RaceResultUpdate data);
}