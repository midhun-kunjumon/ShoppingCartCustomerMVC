using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Models.ViewModels.Order;
using ShoppingCartCustomerMVC.Services.API_Service.OrderService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderApiController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{customerId}")]
        public IActionResult GetCustomerOrders(int customerId)
        {
            var authCustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(authCustomerId, out int authenticatedCustomerId))
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            if (customerId != authenticatedCustomerId)
            {
                return Forbid();
            }

            var orders = _orderService.GetCustomerOrders(customerId);

            return Ok(orders);
        }

        [HttpGet("{customerId}/{orderId}")]
        public IActionResult GetCustomerOrderById(
            int customerId,
            int orderId)
        {
            var authCustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(authCustomerId, out int authenticatedCustomerId))
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            if (customerId != authenticatedCustomerId)
            {
                return Forbid();
            }

            var order = _orderService.GetCustomerOrderById(
                customerId,
                orderId);

            if (order == null)
            {
                return NotFound(new
                {
                    Message = "Order not found."
                });
            }

            return Ok(order);
        }

        [HttpPost]
        public IActionResult CreateOrder(
            CreateOrderViewModel orderViewModel)
        {
            var authCustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(authCustomerId, out int authenticatedCustomerId))
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            if (orderViewModel == null)
            {
                return BadRequest(new { Message = "Invalid order details." });
            }

            orderViewModel.CustomerID = authenticatedCustomerId;

            var result = _orderService.CreateOrder(orderViewModel);

            if (result.OrderID == null)
            {
                return BadRequest(new
                {
                    Message = result.ErrorMessage
                });
            }

            return Ok(new
            {
                OrderID = result.OrderID,
                Message = "Order created successfully."
            });
        }

        [HttpPost("cancel")]
        public IActionResult CancelOrder([FromBody] CancelOrderRequest request)
        {
            var authCustomerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authCustomerId) || authCustomerId != request.CustomerID.ToString())
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            var result = _orderService.CancelCustomerOrder(request.CustomerID, request.OrderID);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new
            {
                Message = result.Message
            });
        }

        [HttpPut("status")]
        public IActionResult UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            var authCustomerIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authCustomerIdStr) || !int.TryParse(authCustomerIdStr, out int authCustomerId))
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            var order = _orderService.GetCustomerOrderById(authCustomerId, request.OrderID);
            if (order == null)
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            var updated = _orderService.UpdateOrderStatus(request.OrderID, request.OrderStatus);
            if (!updated)
            {
                return BadRequest(new { Message = "Failed to update order status." });
            }

            return Ok(new { Message = "Order status updated successfully." });
        }
    }

    public class CancelOrderRequest
    {
        public int CustomerID { get; set; }
        public int OrderID { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public int OrderID { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}