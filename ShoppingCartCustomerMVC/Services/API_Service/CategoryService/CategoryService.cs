using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.CategoryService
{
    public partial class CategoryService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CategoryService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }
}