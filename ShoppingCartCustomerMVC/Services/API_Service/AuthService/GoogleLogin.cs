using Hangfire;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;
using ShoppingCartCustomerMVC.Services.BackgroundService;

namespace ShoppingCartCustomerMVC.Services.API_Service.AuthService
{
    public partial class AuthService
    {
        public string? GoogleLogin(
            string googleId,
            string email,
            string name)
        {
            var customer = _customerService.GetCustomerByGoogleId(googleId);

            if (customer != null)
            {
                return GenerateToken(customer);
            }

            customer = _customerService.GetCustomerByEmail(email);

            if (customer != null)
            {
                return null;
            }

            var newCustomer = new CustomerViewModel
            {
                CustomerName = name,
                Email = email,
                Phone = null,
                City = null,
                PasswordHash = null,
                AuthProvider = "Google",
                GoogleId = googleId
            };

            var customerId = _customerService.CreateCustomer(newCustomer);

            newCustomer.CustomerID = customerId;

            BackgroundJob.Enqueue<HangfireEmailJob>(
                job => job.SendRegistrationAcknowledgement(
                    newCustomer.CustomerName,
                    newCustomer.Email));

            return GenerateToken(newCustomer);
        }
    }
}