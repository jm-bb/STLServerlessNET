using MySql.Data.MySqlClient;

namespace STLServerlessNET;

public class ServiceDatabaseService
{ 
    public MySqlConnection Connection { get; }

    public ServiceDatabaseService(MySqlConnection connection)
    {
        Connection = connection;
    }
}

public class WebDatabaseService
{
    public MySqlConnection Connection { get; }

    public WebDatabaseService(MySqlConnection connection)
    {
        Connection = connection;
    }
}