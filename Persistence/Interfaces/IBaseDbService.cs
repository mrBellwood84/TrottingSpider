namespace Persistence.Interfaces;

public interface IBaseDbService<TModel>
{
    public Task<List<TModel>> GetAllAsync();
    public Task AddAsync(TModel model);
    public Task AddAsync(List<TModel> models);
}