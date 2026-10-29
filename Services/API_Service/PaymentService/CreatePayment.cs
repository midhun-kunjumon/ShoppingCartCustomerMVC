using Dapper;
using ShoppingCartCustomerMVC.Models.ViewModels.Payment;
using System.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.PaymentService
{
    public partial class PaymentService
    {
        public (int? PaymentID, string? ErrorMessage) CreatePayment(
            PaymentViewModel paymentViewModel)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@OrderID", paymentViewModel.OrderID);
            parameters.Add("@PaymentMethod", paymentViewModel.PaymentMethod);
            parameters.Add("@TransactionID", paymentViewModel.TransactionID);
            if (!string.IsNullOrEmpty(paymentViewModel.PaymentStatus))
            {
                parameters.Add("@PaymentStatus", paymentViewModel.PaymentStatus);
            }

            var result = connection.QueryFirstOrDefault(
                "CreateCustomerPayment",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return (null, "Unable to create payment.");
            }

            int? paymentId = result.PaymentID;
            string? errorMessage = result.ErrorMessage;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return (null, errorMessage);
            }

            return (paymentId, null);
        }
    }
}