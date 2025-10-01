using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices.Services;

public interface IRaceResultDataService
{
    Task UpdateAsync(RaceResultUpdate data);
    Task InitCache();
    bool CheckExists(string key);
    RaceResult GetModel(string key);
    Task AddAsync(RaceResult model);
    Task AddAsync(List<RaceResult> models);
}