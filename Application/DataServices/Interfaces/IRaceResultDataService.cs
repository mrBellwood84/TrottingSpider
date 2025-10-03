using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices.Interfaces;

public interface IRaceResultDataService
{
    Task AddBulkAsync(List<RaceResult> data);
    Task UpdateAsync(RaceResultUpdate data);
    Task InitCache();
    bool CheckExists(string key);
    RaceResult GetModel(string key);
    Task AddAsync(RaceResult model);
}