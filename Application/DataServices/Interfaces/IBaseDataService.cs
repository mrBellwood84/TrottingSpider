namespace Application.DataServices.Interfaces;

public interface IBaseDataService<TModel>
{
    public Task InitCache();
    public bool CheckExists(string key);
    public TModel GetModel(string key);
    public Task AddAsync(TModel model);
}