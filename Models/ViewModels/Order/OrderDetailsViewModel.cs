namespace ShoppingCartCustomerMVC.Models.ViewModels.Order
{
    public class OrderDetailsViewModel
    {
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public int CustomerID { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? City { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
            = new List<OrderItemViewModel>();
    }
}