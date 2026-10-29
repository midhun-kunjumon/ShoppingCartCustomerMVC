using Hangfire;
using ShoppingCartCustomerMVC.Data;
using ShoppingCartCustomerMVC.Services.API_Service.AuthService;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly PasswordHasher _passwordHasher;

        public CustomerService(
            DbConnectionFactory connectionFactory,
            IBackgroundJobClient backgroundJobClient,
            PasswordHasher passwordHasher)
        {
            _connectionFactory = connectionFactory;
            _backgroundJobClient = backgroundJobClient;
            _passwordHasher = passwordHasher;
        }
    }
}