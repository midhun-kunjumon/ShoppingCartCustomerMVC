using Microsoft.Data.SqlClient;
using System.Data;

namespace ShoppingCartCustomerMVC.Data
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("ShoppingCartConnection"));
        }
    }
}