using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("[controller]")]
public class WebController(WebDbContext webContext, ILogger<WebController> logger) : ControllerBase
{
    private readonly WebDbContext _webContext = webContext;
    private readonly ILogger<WebController> _logger = logger;

    [HttpGet]
    [Route("zones")]
    public async Task<ActionResult<IEnumerable<Zone>>> GetZones()
    {
        try
        {
            _logger.LogInformation("Calling GetZones()...");
            List<Zone> Zones = await _webContext.Zones.ToListAsync();
            _logger.LogInformation("Zone Count:{@ZoneCount}", Zones.Count);

            return Ok(Zones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving zones.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}