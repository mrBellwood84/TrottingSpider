using Models.DbModels;
using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface IRaceStartNumberDbServiceExtension
{
    /// <summary>
    /// Update single model
    /// </summary>
    Task UpdateAsync(RaceStartNumberUpdate data);

    /// <summary>
    /// Update list of models
    /// </summary>
    Task InsertBulkAsync(List<RaceStartNumber> data);

    /// <summary>
    /// Update start number data with driver id
    /// </summary>
    Task UpdateDriverAsync(RaceStartNumberUpdateDriver data);

    /// <summary>
    /// Updates a list of data with driver id
    /// </summary>
    Task UpdateBulkDriversAsync(List<RaceStartNumberUpdateDriver> data);

    /// <summary>
    /// Update start number data with horse id
    /// </summary>
    Task UpdateHorseAsync(RaceStartNumberUpdateHorse values);

    /// <summary>
    /// Updates a list of data with horse id
    /// </summary>1
    Task UpdateHorsesBulkAsync(List<RaceStartNumberUpdateHorse> data);
}