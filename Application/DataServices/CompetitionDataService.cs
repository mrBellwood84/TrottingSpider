using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class CompetitionDataService(IBaseDbService<Competition>  dbService, IBaseCacheService<Competition> cacheService) : ICompetitionDataService
{
    public async Task InitCache()
    {
        var data = await dbService.GetAllAsync();
        cacheService.InitCache(data);
    }

    public bool CheckCompetitionExists(string key)
    {
        return cacheService.CheckKeyExists(key);
    }

    public bool CheckCompetitionExists(string raceCourseId, string date)
    {
        var key = $"{raceCourseId}_{date}";
        return cacheService.CheckKeyExists(key);
    }

    public bool CheckCompetitionExists(Competition data)
    {
        var key = $"{data.RaceCourseId}_{data.Date}";
        return cacheService.CheckKeyExists(key);
    }

    public Competition GetCompetition(string key)
    {
        return cacheService.GetModel(key);
    }
    
    public async Task AddNewCompetition(Competition newCompetition)
    {
        await dbService.CreateAsync(newCompetition);
        cacheService.AddSingle(newCompetition);
    }
}