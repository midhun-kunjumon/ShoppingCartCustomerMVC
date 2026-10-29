using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.CartService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartApiController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartApiController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{customerId}")]
        public IActionResult GetCart(int customerId)
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

            var cart = _cartService.GetCart(customerId);

            return Ok(cart);
        }

        [HttpPost("add")]
        public IActionResult AddToCart(
            int customerId,
            int productId,
            int quantity)
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

            if (quantity <= 0)
            {
                return BadRequest(new
                {
                    Message = "Quantity must be greater than zero."
                });
            }

            var result = _cartService.AddToCart(
                customerId,
                productId,
                quantity);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Product not found."
                });
            }

            return Ok(new
            {
                Message = "Product added to cart successfully."
            });
        }

        [HttpPut("update")]
        public IActionResult UpdateCart(
            int customerId,
            int cartItemId,
            int quantity)
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

            if (quantity <= 0)
            {
                return BadRequest(new
                {
                    Message = "Quantity must be greater than zero."
                });
            }

            var result = _cartService.UpdateCart(
                customerId,
                cartItemId,
                quantity);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Cart item not found."
                });
            }

            return Ok(new
            {
                Message = "Cart updated successfully."
            });
        }

        [HttpDelete("remove")]
        public IActionResult RemoveFromCart(
            int customerId,
            int cartItemId)
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

            var result = _cartService.RemoveFromCart(
                customerId,
                cartItemId);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Cart item not found."
                });
            }

            return Ok(new
            {
                Message = "Product removed from cart successfully."
            });
        }

        [HttpDelete("clear/{customerId}")]
        public IActionResult ClearCart(int customerId)
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

            var result = _cartService.ClearCart(customerId);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Cart is already empty."
                });
            }

            return Ok(new
            {
                Message = "Cart cleared successfully."
            });
        }

        [HttpPost("merge/{customerId}")]
        public IActionResult MergeCart(int customerId, [FromBody] List<AnonymousCartItemDto> anonymousItems)
        {
            var authCustomerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authCustomerId) || authCustomerId != customerId.ToString())
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            _cartService.MergeCart(customerId, anonymousItems);

            return Ok(new { Message = "Cart merged successfully." });
        }
    }
}