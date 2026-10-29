using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartCustomerMVC.Controllers.MVC_Controller
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}