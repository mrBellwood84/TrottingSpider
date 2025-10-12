using Application.CacheServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class CompetitionDataService(
    IBaseDbService<Competition> dbService,
    ICompetitionDbServiceExtension dbServiceExtension,
    IBaseCacheService<Competition> cacheService)
    : BaseDataService<Competition>(dbService, cacheService), ICompetitionDataService
{
    public async Task UpdateCompetitionStartlistFromSource(CompetitionUpdateStartlistSource data)
    {
        await dbServiceExtension.UpdateStartListFromSource(data);
    }

    public async Task UpdateCompetitionResultsFromSource(CompetitionUpdateResultSource data)
    {
        await dbServiceExtension.UpdateResultsFromSource(data);
    }
}