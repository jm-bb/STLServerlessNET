using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace STLServerlessNET.Controllers.Service;

[ApiController]
[Route("[controller]")]
public class ServiceController(MySqlConnectionFactory connectionFactory, ILogger<ServiceController> logger) : ControllerBase
{
    private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;
    private readonly ILogger<ServiceController> _logger = logger;

    [HttpGet("carriers")]
    public async Task<IActionResult> GetCarriers()
    {
        _logger.LogInformation("Calling GetCarriers()...");

        DataSet ds = new DataSet("Carriers");

        try
        {
            var connection = _connectionFactory.CreateConnection("ServiceConnection");
            await connection.OpenAsync();

            MySqlDataAdapter da = new MySqlDataAdapter(SqlQueries.Carriers, connection);
            da.Fill(ds);

            string carrierJson = JsonConvert.SerializeObject(ds.Tables[0]);
            await connection.CloseAsync();
            return Ok(carrierJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
