using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IHorseDataService
{
    Task<List<Horse>> GetHorseFromDb(string sourceId);
    Task InitCache();
    bool CheckExists(string key);
    Horse GetModel(string key);
    Task AddAsync(Horse model);
}