using MySql.Data.MySqlClient;

public class MySqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MySqlConnection CreateConnection(string name)
    {
        var connectionString = _configuration.GetConnectionString(name);
        return new MySqlConnection(connectionString);
    }
}