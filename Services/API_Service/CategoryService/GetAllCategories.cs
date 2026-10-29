using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Category;

namespace ShoppingCartCustomerMVC.Services.API_Service.CategoryService
{
    public partial class CategoryService
    {
        public List<CategoryViewModel> GetAllCategories()
        {
            using var connection = _connectionFactory.CreateConnection();

            var categories = connection.Query<CategoryViewModel>(
                "GetAllCategories",
                commandType: CommandType.StoredProcedure);

            return categories.ToList();
        }
    }
}