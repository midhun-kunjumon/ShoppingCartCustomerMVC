using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.ProductService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductApiController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound(new
                {
                    Message = "Product not found."
                });
            }

            return Ok(product);
        }

        [HttpGet("subcategory/{subCategoryId}")]
        public IActionResult GetProductsBySubCategoryId(int subCategoryId)
        {
            var products =
                _productService.GetProductsBySubCategoryId(subCategoryId);

            return Ok(products);
        }

        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string query)
        {
            var products = _productService.SearchProducts(query ?? string.Empty);

            return Ok(products);
        }
    }
}