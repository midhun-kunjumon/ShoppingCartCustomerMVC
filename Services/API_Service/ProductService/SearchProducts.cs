using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Product;

namespace ShoppingCartCustomerMVC.Services.API_Service.ProductService
{
    public partial class ProductService
    {
        public List<ProductViewModel> SearchProducts(string query)
        {
            using var connection = _connectionFactory.CreateConnection();

            var products = connection.Query<ProductViewModel>(
                "SearchProducts",
                new { Query = query ?? string.Empty },
                commandType: CommandType.StoredProcedure);

            return products.ToList();
        }
    }
}
