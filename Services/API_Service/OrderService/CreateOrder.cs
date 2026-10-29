using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Order;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        public (int? OrderID, string? ErrorMessage) CreateOrder(
            CreateOrderViewModel orderViewModel)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", orderViewModel.CustomerID);
            if (!string.IsNullOrEmpty(orderViewModel.OrderStatus))
            {
                parameters.Add("@OrderStatus", orderViewModel.OrderStatus);
            }

            var result = connection.QueryFirstOrDefault(
                "CreateCustomerOrder",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return (null, "Unable to create order.");
            }

            int? orderId = result.OrderID;
            string? errorMessage = result.ErrorMessage;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return (null, errorMessage);
            }

            return (orderId, null);
        }
    }
}