using Dapper;
using MySql.Data.MySqlClient;

namespace Persistence;

public class BaseDbService<TModel>
{
    protected string Query { get; init; }
    protected string InsertCommand { get; init; }
    
    private readonly string _connectionString;

    protected BaseDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    protected async Task<List<TModel>> GetAllAsync()
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<TModel>(Query);
        return result.ToList();
    }

    public virtual Task<Dictionary<string, string>> GetIdDictionary()
    {
        throw new NotImplementedException();
    }

    public async Task CreateAsync(TModel model)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(InsertCommand, model);
    }
}