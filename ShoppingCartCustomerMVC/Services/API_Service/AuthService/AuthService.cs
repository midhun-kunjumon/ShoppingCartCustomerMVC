using ShoppingCartCustomerMVC.Data;
using ShoppingCartCustomerMVC.Services.API_Service.CustomerService;
using ShoppingCartCustomerMVC.Services.BackgroundService;
using CustomerServiceClass = ShoppingCartCustomerMVC.Services.API_Service.CustomerService.CustomerService;

namespace ShoppingCartCustomerMVC.Services.API_Service.AuthService
{
    public partial class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly CustomerServiceClass _customerService;
        private readonly PasswordHasher _passwordHasher;
        private readonly DbConnectionFactory _connectionFactory;
        private readonly HangfireEmailJob _hangfireEmailJob;

        public AuthService(
            IConfiguration configuration,
            CustomerServiceClass customerService,
            PasswordHasher passwordHasher,
            DbConnectionFactory connectionFactory,
            HangfireEmailJob hangfireEmailJob)
        {
            _configuration = configuration;
            _customerService = customerService;
            _passwordHasher = passwordHasher;
            _connectionFactory = connectionFactory;
            _hangfireEmailJob = hangfireEmailJob;
        }
    }
}