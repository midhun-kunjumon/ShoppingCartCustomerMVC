using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingCartCustomerMVC.Controllers.MVC_Controller
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            ViewBag.CustomerId = customerId ?? string.Empty;

            return View();
        }
    }
}