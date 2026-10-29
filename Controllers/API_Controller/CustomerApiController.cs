using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;
using ShoppingCartCustomerMVC.Services.API_Service.CustomerService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerApiController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerApiController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Invalid registration details." });
            }

            var (customerId, errorMessage) = _customerService.RegisterCustomer(model);
            if (errorMessage != null)
            {
                return BadRequest(new
                {
                    Message = errorMessage
                });
            }

            return Ok(new
            {
                CustomerID = customerId,
                Message = "Customer registered successfully."
            });
        }
        

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = "Customer not found."
                });
            }

            customer.PasswordHash = null;

            return Ok(customer);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, CustomerViewModel customer)
        {
            customer.CustomerID = id;

            var updated = _customerService.UpdateCustomer(customer);

            if (!updated)
            {
                return NotFound(new
                {
                    Message = "Customer not found."
                });
            }

            return Ok(new
            {
                Message = "Customer updated successfully."
            });
        }
    }
}