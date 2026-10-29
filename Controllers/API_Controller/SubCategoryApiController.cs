using Microsoft.AspNetCore.Mvc;
using ShoppingCartCustomerMVC.Services.API_Service.SubCategoryService;

namespace ShoppingCartCustomerMVC.Controllers.API_Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryApiController : ControllerBase
    {
        private readonly SubCategoryService _subCategoryService;

        public SubCategoryApiController(SubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpGet("category/{categoryId}")]
        public IActionResult GetSubCategoriesByCategoryId(int categoryId)
        {
            var subCategories =
                _subCategoryService.GetSubCategoriesByCategoryId(categoryId);

            return Ok(subCategories);
        }

        [HttpGet("{id}")]
        public IActionResult GetSubCategoryById(int id)
        {
            var subCategory = _subCategoryService.GetSubCategoryById(id);

            if (subCategory == null)
            {
                return NotFound(new
                {
                    Message = "SubCategory not found."
                });
            }

            return Ok(subCategory);
        }
    }
}