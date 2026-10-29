using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Category;

namespace ShoppingCartCustomerMVC.Services.API_Service.CategoryService
{
    public partial class CategoryService
    {
        public CategoryViewModel? GetCategoryById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<CategoryViewModel>(
                "GetCategoryById",
                new
                {
                    CategoryId = id
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}