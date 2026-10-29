using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.AuthService;
using System.Security.Claims;

namespace ShoppingCartCustomerMVC.Controllers.MVC_Controller
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
      
        [HttpPost]
        public async Task<IActionResult> CompleteLogin(string token, string refreshToken, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(token) ||
                string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("Login");
            }

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var customerId = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            var name = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)
                ?.Value;

            var email = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;

            if (string.IsNullOrEmpty(customerId) ||
                string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            Response.Cookies.Append(
                "jwtToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });
            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

            var customerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customerId),
                new Claim(ClaimTypes.Name, name ?? email),
                new Claim(ClaimTypes.Email, email)
            };

            var identity = new ClaimsIdentity(
                customerClaims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var targetUrl = (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? returnUrl : "/Home/Index";
                return Ok(new { customerId, returnUrl = targetUrl });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Account", new { returnUrl })
            };

            return Challenge(
                properties,
                GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Claims;

            var googleId = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            var email = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;

            var name = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)
                ?.Value;

            if (string.IsNullOrEmpty(googleId) ||
                string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var token = _authService.GoogleLogin(
                googleId,
                email,
                name ?? email);

            if (token == null)
            {
                return RedirectToAction("Login");
            }

            Response.Cookies.Append(
                "jwtToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var customerId = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            var customerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customerId ?? googleId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name ?? email)
            };

            var identity = new ClaimsIdentity(
                customerClaims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwtToken");

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Profile()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }
            ViewBag.CustomerId = customerId;
            return View();
        }
    }
}