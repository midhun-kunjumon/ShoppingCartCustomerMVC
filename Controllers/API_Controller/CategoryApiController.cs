using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.CategoryService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryApiController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategories();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound(new
                {
                    Message = "Category not found."
                });
            }

            return Ok(category);
        }
    }
}