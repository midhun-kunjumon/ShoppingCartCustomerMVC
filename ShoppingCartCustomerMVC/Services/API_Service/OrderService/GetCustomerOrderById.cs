using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Order;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        public OrderDetailsViewModel? GetCustomerOrderById(
            int customerId,
            int orderId)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OrderID", orderId);
            parameters.Add("@CustomerID", customerId);

            using var multi = connection.QueryMultiple(
                "GetCustomerOrderById",
                parameters,
                commandType: CommandType.StoredProcedure);

            var order = multi.ReadFirstOrDefault<OrderDetailsViewModel>();

            if (order == null)
            {
                return null;
            }

            order.OrderItems = multi
                .Read<OrderItemViewModel>()
                .ToList();

            return order;
        }
    }
}