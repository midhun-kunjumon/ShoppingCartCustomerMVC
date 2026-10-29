namespace ShoppingCartCustomerMVC.Models.ViewModels.Order
{
    public class CreateOrderViewModel
    {
        public int CustomerID { get; set; }

        public string? PaymentMethod { get; set; }

        public string? OrderStatus { get; set; }
    }
}