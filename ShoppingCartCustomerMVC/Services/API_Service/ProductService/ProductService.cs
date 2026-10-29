using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.ProductService
{
    public partial class ProductService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ProductService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }
}