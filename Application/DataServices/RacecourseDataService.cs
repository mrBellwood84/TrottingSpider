using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class RacecourseDataService(
    IBaseDbService<Racecourse> dbService, 
    IBaseCacheService<Racecourse> cacheService) 
    : BaseDataService<Racecourse>(dbService, cacheService)
{ }