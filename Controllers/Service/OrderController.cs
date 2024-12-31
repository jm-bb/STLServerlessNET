using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Controllers.Service;

[ApiController]
[Route("[controller]")]
public class OrderController(ServiceDbContext serviceDbContext, ILogger<OrderController> logger) : ControllerBase
{
    private readonly ServiceDbContext _serviceDbContext = serviceDbContext;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpGet]
    [Route("{id}")]
    //public async Task<IActionResult> GetOrderDetails()
    public async Task<ActionResult<int>> GetOrderDetails(int id)
    {
        _logger.LogInformation("Calling GetOrderDetails()...");
        _logger.LogInformation("Order ID:{@orderId}", id);

        return Ok(id);
        //// Query the order by its ID and include the related User
        //var order = await _serviceDbContext.Orders
        //                          .Include(o => o.User) // Include the related User
        //                          .FirstOrDefaultAsync(o => o.OrderId == id);

        //if (order == null)
        //{
        //    return NotFound(new { Message = $"Order with ID {id} not found." });
        //}

        //// Return the order data
        //return Ok(new
        //{
        //    order.OrderId,
        //    order.CreateDate,
        //    User = new
        //    {
        //        order.UserId,
        //        order.User.FirstName,
        //        order.User.LastName
        //    }
        //});
    }
}