namespace Application.DataServices;

public interface IBaseDataService<TModel>
{
    public Task InitCache();
    public bool CheckExists(string key);
    public TModel GetModel(string key);
    public Task AddAsync(TModel model);
    public Task AddBulkAsync(List<TModel> models);
}