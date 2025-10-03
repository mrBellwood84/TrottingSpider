using Models.Settings;
using MySql.Data.MySqlClient;

namespace Persistence;

public class DbConnection(DbConnectionStrings dbConnectionStrings)
{
    private readonly string _connectionString = dbConnectionStrings.Default;

    protected MySqlConnection CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}