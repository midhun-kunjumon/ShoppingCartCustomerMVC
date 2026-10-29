using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.AuthService
{
    public partial class AuthService
    {
        public (string AccessToken, string RefreshToken, DateTime RefreshTokenExpiry)?
            RefreshAccessToken(string refreshToken)
        {
            var storedToken = GetRefreshToken(refreshToken);

            if (storedToken == null)
            {
                return null;
            }

            if (storedToken.RevokedAt != null)
            {
                return null;
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            var customer = _customerService.GetCustomerById(
                storedToken.CustomerID);

            if (customer == null)
            {
                return null;
            }

            // Revoke old refresh token
            RevokeRefreshToken(refreshToken);

            // Generate new tokens
            var accessToken = GenerateToken(customer);

            var newRefreshToken = GenerateRefreshToken();

            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            SaveRefreshToken(
                customer.CustomerID,
                newRefreshToken,
                refreshTokenExpiry);

            return (
                accessToken,
                newRefreshToken,
                refreshTokenExpiry);
        }
    }
}