using Dapper;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.OrderService
{
    public partial class OrderService
    {
        public bool UpdateOrderStatus(int orderId, string orderStatus)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OrderID", orderId);
            parameters.Add("@OrderStatus", orderStatus);

            var rows = connection.Execute(
                "UpdateOrderStatus",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }
    }
}
