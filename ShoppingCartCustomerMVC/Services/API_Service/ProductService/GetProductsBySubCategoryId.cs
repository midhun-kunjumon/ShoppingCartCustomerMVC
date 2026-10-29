using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Product;

namespace ShoppingCartCustomerMVC.Services.API_Service.ProductService
{
    public partial class ProductService
    {
        public List<ProductViewModel> GetProductsBySubCategoryId(int subCategoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var products = connection.Query<ProductViewModel>(
                "GetProductsBySubCategoryId",
                new
                {
                    SubCategoryId = subCategoryId
                },
                commandType: CommandType.StoredProcedure);

            return products.ToList();
        }
    }
}