namespace ShoppingCartCustomerMVC.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public int CartItemID { get; set; }

        public int CartID { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public string? ImageUrl { get; set; }
    }
}