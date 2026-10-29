using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Payment;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.PaymentService
{
    public partial class PaymentService
    {
        public PaymentViewModel? GetPaymentById(int paymentId)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PaymentID", paymentId);

            return connection.QueryFirstOrDefault<PaymentViewModel>(
                "GetPaymentById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}