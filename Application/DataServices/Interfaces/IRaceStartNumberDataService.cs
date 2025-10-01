using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices.Interfaces;

public interface IRaceStartNumberDataService
{
    Task UpdateDriverAsync(RaceStartNumberUpdateDriver data);
    Task UpdateHorseAsync(RaceStartNumberUpdateHorse data);
    Task UpdateAsync(RaceStartNumberUpdate data);
    Task InitCache();
    bool CheckExists(string key);
    RaceStartNumber GetModel(string key);
    Task AddAsync(RaceStartNumber model);
    Task AddAsync(List<RaceStartNumber> models);
}