using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;

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

        DataSet ds = new DataSet("EligibleOrders");

        try
        {
            await _serviceConnection.OpenAsync();
            return Ok("carriers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving carriers.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}