using MySql.Data.MySqlClient;

namespace STLServerlessNET.Helpers
{
    public class MySqlConnectionFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public MySqlConnection CreateConnection(string name)
        {
            var connectionString = _configuration.GetConnectionString(name);
            return new MySqlConnection(connectionString);
        }
    }
}