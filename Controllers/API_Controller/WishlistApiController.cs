using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.WishlistService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistApiController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistApiController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("{customerId}")]
        public IActionResult GetWishlist(int customerId)
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

            var wishlist = _wishlistService.GetWishlist(customerId);

            return Ok(wishlist);
        }

        [HttpPost("add")]
        public IActionResult AddToWishlist(
            int customerId,
            int productId)
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

            var result = _wishlistService.AddToWishlist(
                customerId,
                productId);

            if (!result)
            {
                return BadRequest(new
                {
                    Message = "Product could not be added to wishlist. It may not exist or may already be in the wishlist."
                });
            }

            return Ok(new
            {
                Message = "Product added to wishlist successfully."
            });
        }

        [HttpDelete("remove")]
        public IActionResult RemoveFromWishlist(
            int customerId,
            int wishlistId)
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

            var result = _wishlistService.RemoveFromWishlist(
                customerId,
                wishlistId);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Wishlist item not found."
                });
            }

            return Ok(new
            {
                Message = "Product removed from wishlist successfully."
            });
        }

        [HttpPost("merge/{customerId}")]
        public IActionResult MergeWishlist(int customerId, [FromBody] List<int> productIds)
        {
            var authCustomerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authCustomerId) || authCustomerId != customerId.ToString())
            {
                return Unauthorized(new { Message = "Unauthorized access." });
            }

            _wishlistService.MergeWishlist(customerId, productIds);

            return Ok(new { Message = "Wishlist merged successfully." });
        }
    }
}