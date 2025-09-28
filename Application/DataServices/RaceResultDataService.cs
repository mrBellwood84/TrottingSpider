using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class RaceResultDataService(
    IBaseDbService<RaceResult> dbService, 
    IBaseCacheService<RaceResult> cacheService) 
    : BaseDataService<RaceResult>(dbService, cacheService)
{ }