using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Auth;
using System.Security.Cryptography;

namespace ShoppingCartCustomerMVC.Services.API_Service.AuthService
{
    public partial class AuthService
    {
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public void SaveRefreshToken(
            int customerId,
            string refreshToken,
            DateTime expiresAt)
        {
            using var connection = _connectionFactory.CreateConnection();

            connection.Execute(
                "InsertRefreshToken",
                new
                {
                    CustomerID = customerId,
                    Token = refreshToken,
                    ExpiresAt = expiresAt
                },
                commandType: CommandType.StoredProcedure);
        }

        public RefreshTokenViewModel? GetRefreshToken(
            string refreshToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<RefreshTokenViewModel>(
                "GetRefreshToken",
                new
                {
                    Token = refreshToken
                },
                commandType: CommandType.StoredProcedure);
        }

        public void RevokeRefreshToken(
            string refreshToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            connection.Execute(
                "RevokeRefreshToken",
                new
                {
                    Token = refreshToken
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}