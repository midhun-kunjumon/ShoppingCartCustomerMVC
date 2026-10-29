using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public CustomerViewModel? GetCustomerByEmail(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<CustomerViewModel>(
                "GetCustomerByEmail",
                new
                {
                    Email = email
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}