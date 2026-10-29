namespace ShoppingCartCustomerMVC.Models.ViewModels.Payment
{
    public class PaymentViewModel
    {
        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? TransactionID { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}