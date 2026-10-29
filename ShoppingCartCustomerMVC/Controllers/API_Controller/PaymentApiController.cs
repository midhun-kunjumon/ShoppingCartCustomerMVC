using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Models.ViewModels.Payment;
using ShoppingCartCustomerMVC.Services.API_Service.OrderService;
using ShoppingCartCustomerMVC.Services.API_Service.PaymentService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentApiController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;

        public PaymentApiController(
            PaymentService paymentService,
            OrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpGet("{paymentId}")]
        public IActionResult GetPaymentById(int paymentId)
        {
            var authCustomerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(authCustomerIdStr, out int authenticatedCustomerId))
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            var payment = _paymentService.GetPaymentById(paymentId);

            if (payment == null)
            {
                return NotFound(new
                {
                    Message = "Payment not found."
                });
            }

            var order = _orderService.GetCustomerOrderById(authenticatedCustomerId, payment.OrderID);
            if (order == null)
            {
                return Forbid();
            }

            return Ok(payment);
        }

        [HttpPost]
        public IActionResult CreatePayment(
            PaymentViewModel paymentViewModel)
        {
            var result = _paymentService.CreatePayment(paymentViewModel);

            if (result.PaymentID == null)
            {
                return BadRequest(new
                {
                    Message = result.ErrorMessage
                });
            }

            return Ok(new
            {
                PaymentID = result.PaymentID,
                Message = "Payment created successfully."
            });
        }
    }
}