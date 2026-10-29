using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Product;

namespace ShoppingCartCustomerMVC.Services.API_Service.ProductService
{
    public partial class ProductService
    {
        public ProductViewModel? GetProductById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<ProductViewModel>(
                "GetProductById",
                new
                {
                    ProductId = id
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}