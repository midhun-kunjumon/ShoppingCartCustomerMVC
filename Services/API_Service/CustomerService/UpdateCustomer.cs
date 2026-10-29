using System.Data;
using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Customer;

namespace ShoppingCartCustomerMVC.Services.API_Service.CustomerService
{
    public partial class CustomerService
    {
        public bool UpdateCustomer(CustomerViewModel customer)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.Execute(
                "UpdateCustomer",
                new
                {
                    CustomerId = customer.CustomerID,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City
                },
                commandType: CommandType.StoredProcedure) > 0;
        }
    }
}