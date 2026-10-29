namespace ShoppingCartCustomerMVC.Models.ViewModels.Order
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }
    }
}