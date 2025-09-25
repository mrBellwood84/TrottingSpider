using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class RaceCourseDataService(IBaseDbService<RaceCourse> dbService, IBaseCacheService<RaceCourse> cacheService) : IRaceCourseDataService
{    
    public async Task InitCache()
     {
         var data = await dbService.GetAllAsync();
         cacheService.InitCache(data);
     }
    public bool CheckRaceCourseExists(string raceCourseName)
    {
        return cacheService.CheckKeyExists(raceCourseName);
    }
    public bool CheckRaceCourseExists(RaceCourse raceCourse)
    {
        return cacheService.CheckKeyExists(raceCourse);
    }

    public RaceCourse GetRaceCourse(string raceCourseName)
    {
        return cacheService.GetModel(raceCourseName);
    }

    public async Task AddNewRaceCourse(RaceCourse newRaceCourse)
    {
        await dbService.CreateAsync(newRaceCourse);
        cacheService.AddSingle(newRaceCourse);
    }
}