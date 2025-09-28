using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class RacecourseDataService(
    IBaseDbService<Racecourse> dbService, 
    IBaseCacheService<Racecourse> cacheService) 
    : BaseDataService<Racecourse>(dbService, cacheService)
{ }