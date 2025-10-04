using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IDriverDataService
{
    Task<List<Driver>> GetDriverFromDb(string sourceId);
    Task AddDriverToCacheAsync(string sourceId);
    Task InitDriverCacheAsync();
    bool CheckExists(string key);
    Driver GetModel(string key);
    Task AddAsync(Driver model);
}