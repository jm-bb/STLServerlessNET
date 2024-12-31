using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace STLServerlessNET.Controllers.Service;

[ApiController]
[Route("[controller]")]
public class ServiceController : ControllerBase
{
    private readonly ILogger<ServiceController> _logger;
    private readonly MySqlConnection _serviceConnection;

    public ServiceController(MySqlConnection serviceConnection, ILogger<ServiceController> logger)
    {
        _serviceConnection = serviceConnection;
        _logger = logger;
    }

    [HttpGet("carriers")]
    public async Task<IActionResult> GetCarriers()
    {
        _logger.LogInformation("Calling GetCarriers()...");

        DataSet ds = new DataSet("Carriers");

        try
        {
            await _serviceConnection.OpenAsync();
            MySqlDataAdapter da = new MySqlDataAdapter("select * from carrier", _serviceConnection);
            da.Fill(ds);

            string carrierJson = JsonConvert.SerializeObject(ds.Tables[0]);
            return Ok(carrierJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving carriers.");
            return StatusCode(500, "An internal server error occurred.");
        }
        finally
        {
            _serviceConnection.Close();
        }
    }
}