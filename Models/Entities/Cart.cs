namespace ShoppingCartCustomerMVC.Models.Entities
{
    public class Cart
    {
        public int CartID { get; set; }

        public int CustomerID { get; set; }

        public Customer Customer { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}