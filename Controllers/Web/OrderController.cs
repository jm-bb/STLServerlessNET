using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Controllers.Web;

[ApiController]
[Route("web/[controller]")]
public class OrderController(WebDbContext webDbContext, ILogger<OrderController> logger) : ControllerBase
{
    private readonly WebDbContext _webDbContext = webDbContext;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        _logger.LogInformation("Calling GetOrderDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        // Query the order by its ID and include the related User
        var orderWithDetails = await _webDbContext.Orders
            //.Include(o => o.User)
            .Include(o => o.Carts)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (orderWithDetails == null)
        {
            return NotFound(new { Message = $"Order with ID {id} not found." });
        }

        // Return the order data
        return Ok(orderWithDetails);
    }
}
