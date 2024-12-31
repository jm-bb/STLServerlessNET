using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("web/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly MySqlConnection _webConnection;

    public OrderController(MySqlConnection webConnection, ILogger<OrderController> logger)
    {
        _webConnection = webConnection;
        _logger = logger;
    }

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

            return Ok(ds.Tables[0]);
        }
        catch
        {
            throw;
        }
    }
}
