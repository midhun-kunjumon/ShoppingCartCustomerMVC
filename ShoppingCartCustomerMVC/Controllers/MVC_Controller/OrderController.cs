using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingCartCustomerMVC.Controllers.MVC_Controller
{
    [Authorize]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerId = customerId;
            return View();
        }

        public IActionResult Checkout()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerId = customerId;
            return View();
        }

        public IActionResult Success(int orderId)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerId = customerId;
            ViewBag.OrderId = orderId;
            return View();
        }

        public IActionResult Details(int id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerId = customerId;
            ViewBag.OrderId = id;
            return View();
        }

        public IActionResult Payment(int id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerId = customerId;
            ViewBag.OrderId = id;
            return View();
        }
    }
}
