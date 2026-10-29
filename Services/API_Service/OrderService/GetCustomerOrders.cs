using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Order;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        public List<OrderViewModel> GetCustomerOrders(int customerId)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", customerId);

            return connection.Query<OrderViewModel>(
                "GetCustomerOrders",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
    }
}