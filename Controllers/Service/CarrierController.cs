using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace STLServerlessNET.Controllers.Service;

[ApiController]
[Route("[controller]")]
public class ServiceController(ServiceDatabaseService serviceDatabaseService, ILogger<ServiceController> logger) : ControllerBase
{
    private readonly ILogger<ServiceController> _logger = logger;
    private readonly ServiceDatabaseService _serviceDatabaseService = serviceDatabaseService;

    [HttpGet("carriers")]
    public async Task<IActionResult> GetCarriers()
    {
        _logger.LogInformation("Calling GetCarriers()...");

        DataSet ds = new DataSet("Carriers");

        try
        {
            await _serviceDatabaseService.Connection.OpenAsync();
            MySqlDataAdapter da = new MySqlDataAdapter(SqlQueries.Carriers, _serviceDatabaseService.Connection);
            da.Fill(ds);

            string carrierJson = JsonConvert.SerializeObject(ds.Tables[0]);
            await _serviceDatabaseService.Connection.CloseAsync();
            return Ok(carrierJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}