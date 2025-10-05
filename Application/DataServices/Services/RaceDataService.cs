using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class RaceDataService(
    IBaseDbService<Race> dbService,
    IBaseCacheService<Race> cacheService)
    : BaseDataService<Race>(dbService, cacheService);
