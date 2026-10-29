using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public (int? CustomerID, string? ErrorMessage) RegisterCustomer(RegisterViewModel model)
        {
            var existingCustomer = GetCustomerByEmail(model.Email);
            if (existingCustomer != null)
            {
                return (null, "An account with this email address already exists.");
            }

            var customer = new CustomerViewModel
            {
                CustomerName = model.CustomerName,
                Email = model.Email,
                Phone = model.Phone,
                City = model.City,
                PasswordHash = _passwordHasher.HashPassword(model.Password),
                AuthProvider = "Local",
                GoogleId = null
            };

            var customerId = CreateCustomer(customer);

            return (customerId, null);
        }
    }
}
