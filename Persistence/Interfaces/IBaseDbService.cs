namespace Persistence.Interfaces;

public interface IBaseDbService<TModel>
{
    Task<List<TModel>> GetAllAsync();
    Task AddAsync(TModel model);
    Task AddAsync(List<TModel> models);
}