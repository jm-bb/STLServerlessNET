using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("web/[controller]")]
public class CartController(MySqlConnectionFactory connectionFactory, ILogger<CartController> logger) : ControllerBase
{
    private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;
    private readonly ILogger<CartController> _logger = logger;

    [HttpGet("{id}")]
    public IActionResult GetCartDetails(int id)
    {
        _logger.LogInformation("Calling GetCartDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        try
        {
            var connection = _connectionFactory.CreateConnection("WebConnection");

            WebDatabaseHelper wdh = new WebDatabaseHelper(connection);
            DataTable dt = wdh.GetOrderDetails(id);

            string cartJson = JsonConvert.SerializeObject(dt);
            return Ok(cartJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
