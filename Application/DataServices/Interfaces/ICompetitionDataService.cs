using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.DataServices;

public interface ICompetitionDataService
{
    Task UpdateCompetitionStartlistFromSource(CompetitionUpdateStartlistSource data);
    Task UpdateCompetitionResultsFromSource(CompetitionUpdateResultSource data);
    Task InitCache();
    bool CheckExists(string key);
    Competition GetModel(string key);
    Task AddAsync(Competition model);
    Task AddBulkAsync(List<Competition> models);
}