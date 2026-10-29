using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingCartCustomerMVC.Controllers.MVC_Controller
{
    public class ProductController : Controller
    {
        public IActionResult SubCategory(int id)
        {
            ViewBag.SubCategoryId = id;

            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.ProductId = id;

            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            ViewBag.CustomerId = customerId;

            return View();
        }

        public IActionResult Search(string query)
        {
            ViewBag.Query = query;

            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            ViewBag.CustomerId = customerId;

            return View();
        }
    }
}