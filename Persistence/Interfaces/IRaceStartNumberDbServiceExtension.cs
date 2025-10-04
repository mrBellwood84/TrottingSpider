using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface IRaceStartNumberDbServiceExtension
{
    /// <summary>
    /// Update single model
    /// </summary>
    Task UpdateAsync(RaceStartNumberUpdate data);

    /// <summary>
    /// Updates a list of data with driver id
    /// </summary>
    Task BulkUpdateDriversAsync(List<RaceStartNumberUpdateDriver> data);

    /// <summary>
    /// Updates a list of data with horse id
    /// </summary>1
    Task BulkUpdateHorsesAsync(List<RaceStartNumberUpdateHorse> data);
}