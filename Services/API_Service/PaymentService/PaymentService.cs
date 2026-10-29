using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.PaymentService
{
    public partial class PaymentService
    {
        private readonly DbConnectionFactory _db;

        public PaymentService(DbConnectionFactory db)
        {
            _db = db;
        }
    }
}