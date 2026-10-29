using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public CustomerViewModel? GetCustomerById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<CustomerViewModel>(
                "GetCustomerById",
                new
                {
                    CustomerId = id
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}