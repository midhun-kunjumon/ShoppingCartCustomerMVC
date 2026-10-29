using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Category;

namespace ShoppingCartCustomerMVC.Services.API_Service.SubCategoryService
{
    public partial class SubCategoryService
    {
        public SubCategoryViewModel? GetSubCategoryById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<SubCategoryViewModel>(
                "GetSubCategoryById",
                new
                {
                    SubCategoryId = id
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}