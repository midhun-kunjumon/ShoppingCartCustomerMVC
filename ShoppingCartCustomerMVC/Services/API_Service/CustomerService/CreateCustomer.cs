using System.Data;
using Dapper;
using Hangfire;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;
using ShoppingCartCustomerMVC.Services.BackgroundService;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public int CreateCustomer(CustomerViewModel customer)
        {
            using var connection = _connectionFactory.CreateConnection();

            var customerId = connection.QuerySingle<int>(
                "CreateCustomer",
                new
                {
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City,
                    PasswordHash = customer.PasswordHash,
                    AuthProvider = customer.AuthProvider,
                    GoogleId = customer.GoogleId
                },
                commandType: CommandType.StoredProcedure);

            if (customer != null && customer.AuthProvider == "Local" && !string.IsNullOrEmpty(customer.Email))
            {
                _backgroundJobClient.Enqueue<HangfireEmailJob>(
                    job => job.SendRegistrationAcknowledgement(
                        customer.CustomerName,
                        customer.Email));
            }

            return customerId;
        }
    }
}