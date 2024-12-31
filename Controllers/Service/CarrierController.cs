using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Controllers.Service;

[ApiController]
[Route("service/[controller]")]
public class ServiceController(ServiceDbContext serviceDbContext, ILogger<ServiceController> logger) : ControllerBase
{
    private readonly ServiceDbContext _serviceDbContext = serviceDbContext;
    private readonly ILogger<ServiceController> _logger = logger;

    [HttpGet]
    [Route("carriers")]
    public async Task<ActionResult<IEnumerable<Carrier>>> GetCarriers()
    {
        try
        {
            _logger.LogInformation("Calling GetCarriers()...");
            List<Carrier> Carriers = await _serviceDbContext.Carriers.ToListAsync();
            _logger.LogInformation("Carrier Count:{@CarrierCount}", Carriers.Count);


            return Ok(Carriers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving carriers.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}