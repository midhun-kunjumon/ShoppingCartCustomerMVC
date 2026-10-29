using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Models;
using ShoppingCartCustomerMVC.Services.API_Service.AuthService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthApiController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            var result = _authService.Login(
                model.Email,
                model.Password);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                refreshToken = result.Value.RefreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new
                {
                    message = "Refresh token not found."
                });
            }

            var result = _authService.RefreshAccessToken(refreshToken);

            if (result == null)
            {
                Response.Cookies.Delete("refreshToken");

                return Unauthorized(new
                {
                    message = "Invalid or expired refresh token."
                });
            }

            Response.Cookies.Append(
                "refreshToken",
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = result.Value.RefreshTokenExpiry
                });

            return Ok(new
            {
                accessToken = result.Value.AccessToken
            });
        }
    }
}