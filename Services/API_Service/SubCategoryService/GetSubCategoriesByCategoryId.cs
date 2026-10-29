using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Category;

namespace ShoppingCartCustomerMVC.Services.API_Service.SubCategoryService
{
    public partial class SubCategoryService
    {
        public List<SubCategoryViewModel> GetSubCategoriesByCategoryId(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var subCategories = connection.Query<SubCategoryViewModel>(
                "GetSubCategoriesByCategoryId",
                new
                {
                    CategoryId = categoryId
                },
                commandType: CommandType.StoredProcedure);

            return subCategories.ToList();
        }
    }
}