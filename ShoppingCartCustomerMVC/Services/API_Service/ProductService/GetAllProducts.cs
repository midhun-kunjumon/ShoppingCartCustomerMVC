using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Product;

namespace ShoppingCartCustomerMVC.Services.API_Service.ProductService
{
    public partial class ProductService
    {
        public List<ProductViewModel> GetAllProducts()
        {
            using var connection = _connectionFactory.CreateConnection();

            var products = connection.Query<ProductViewModel>(
                "GetAllProducts",
                commandType: CommandType.StoredProcedure);

            return products.ToList();
        }
    }
}