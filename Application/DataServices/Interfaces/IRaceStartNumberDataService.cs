using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices.Services;

public interface IRaceStartNumberDataService
{
    Task BulkAddAsync(List<RaceStartNumber> data);
    Task UpdateAsync(RaceStartNumberUpdate data);
    Task BulkUpdateDriversAsync(List<RaceStartNumberUpdateDriver> data);
    Task BulkUpdateHorsesAsync(List<RaceStartNumberUpdateHorse> data);
    Task InitCache();
    bool CheckExists(string key);
    RaceStartNumber GetModel(string key);
    Task AddAsync(RaceStartNumber model);
}