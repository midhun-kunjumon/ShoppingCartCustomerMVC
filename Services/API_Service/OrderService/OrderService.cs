using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        private readonly DbConnectionFactory _db;

        public OrderService(DbConnectionFactory db)
        {
            _db = db;
        }
    }
}