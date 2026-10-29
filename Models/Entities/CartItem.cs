namespace ShoppingCartCustomerMVC.Models.Entities
{
    public class CartItem
    {
        public int CartItemID { get; set; }

        public int CartID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public Cart Cart { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
