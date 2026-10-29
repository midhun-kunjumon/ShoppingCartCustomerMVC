using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.SubCategoryService
{
    public partial class SubCategoryService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public SubCategoryService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }
}