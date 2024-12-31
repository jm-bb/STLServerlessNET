using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using STLServerlessNET.Controllers.Service;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("web/[controller]")]
public class OrderController(MySqlConnectionFactory connectionFactory, ILogger<OrderController> logger) : ControllerBase
{
    private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        _logger.LogInformation("Calling GetOrderDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        DataSet ds = new DataSet("EligibleOrders");

        try
        {
            var connection = _connectionFactory.CreateConnection("WebConnection");
            await connection.OpenAsync();

            string sql = SqlQueries.EligibleOrder.Replace("_ORDER_ID_", id.ToString());
            MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
            da.Fill(ds);

            string orderJson = JsonConvert.SerializeObject(ds.Tables[0]);
            await connection.CloseAsync();
            return Ok(orderJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
