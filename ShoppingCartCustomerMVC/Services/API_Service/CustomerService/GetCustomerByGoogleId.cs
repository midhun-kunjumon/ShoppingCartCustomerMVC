using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public CustomerViewModel? GetCustomerByGoogleId(string googleId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<CustomerViewModel>(
                "GetCustomerByGoogleId",
                new
                {
                    GoogleId = googleId
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}