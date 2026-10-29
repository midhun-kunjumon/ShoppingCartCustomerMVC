using Dapper;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        public (bool Success, string Message) CancelCustomerOrder(
            int customerId,
            int orderId)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OrderID", orderId);
            parameters.Add("@CustomerID", customerId);

            var result = connection.QueryFirstOrDefault(
                "CancelCustomerOrder",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return (false, "Unable to process order cancellation.");
            }

            int successInt = result.Success != null ? Convert.ToInt32(result.Success) : 0;
            bool success = successInt == 1;
            string message = result.Message ?? "Order cancellation processed.";

            return (success, message);
        }
    }
}
