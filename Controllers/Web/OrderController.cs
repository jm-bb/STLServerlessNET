using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("web/[controller]")]
public class OrderController([FromServices] MySqlConnection webConnection, ILogger<OrderController> logger) : ControllerBase
{
    private readonly ILogger<OrderController> _logger = logger;
    private readonly MySqlConnection _webConnection = webConnection;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        _logger.LogInformation("Calling GetOrderDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        DataSet ds = new DataSet("EligibleOrders");

        try
        {
            await _webConnection.OpenAsync();
            string sql = SqlQueries.EligibleOrder.Replace("_ORDER_ID_", id.ToString());
            MySqlDataAdapter da = new MySqlDataAdapter(sql, _webConnection);
            da.Fill(ds);

            string orderJson = JsonConvert.SerializeObject(ds.Tables[0]);
            await _webConnection.CloseAsync();
            return Ok(orderJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
