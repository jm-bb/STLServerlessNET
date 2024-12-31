using System.Data;
using MySql.Data.MySqlClient;
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
    public async Task<IActionResult> GetCartDetails(int id)
    {
        _logger.LogInformation("Calling GetCartDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        DataSet cart = new DataSet("Cart");

        try
        {
            var connection = _connectionFactory.CreateConnection("WebConnection");
            await connection.OpenAsync();

            string sql = SqlQueries.OrderDetails.Replace("_ORDER_ID_", id.ToString());
            MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
            da.Fill(cart);

            string cartJson = JsonConvert.SerializeObject(cart.Tables[0]);

            await connection.CloseAsync();
            return Ok(cartJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
