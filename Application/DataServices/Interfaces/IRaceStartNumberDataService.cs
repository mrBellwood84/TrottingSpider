using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices.Interfaces;

public interface IRaceStartNumberDataService
{
    Task AddBulkAsync(List<RaceStartNumber> data);
    Task UpdateAsync(RaceStartNumberUpdate data);
    Task UpdateDriversBulkAsync(List<RaceStartNumberUpdateDriver> data);
    Task UpdateHorsesBulkAsync(List<RaceStartNumberUpdateHorse> data);
    Task InitCache();
    bool CheckExists(string key);
    RaceStartNumber GetModel(string key);
    Task AddAsync(RaceStartNumber model);
}