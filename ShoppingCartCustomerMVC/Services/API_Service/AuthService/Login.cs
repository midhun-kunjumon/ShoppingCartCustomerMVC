using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.AuthService
{
    public partial class AuthService
    {
        public (string AccessToken, string RefreshToken)? Login(
            string email,
            string password)
        {
            var customer = _customerService.GetCustomerByEmail(email);

            if (customer == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(customer.PasswordHash))
            {
                return null;
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(
                password,
                customer.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            var accessToken = GenerateToken(customer);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            SaveRefreshToken(
                customer.CustomerID,
                refreshToken,
                refreshTokenExpiry);

            return (accessToken, refreshToken);
        }
    }
}