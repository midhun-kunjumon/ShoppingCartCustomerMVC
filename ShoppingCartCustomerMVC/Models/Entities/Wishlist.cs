namespace ShoppingCartCustomerMVC.Models.Entities
{
    public class Wishlist
    {
        public int WishlistID { get; set; }

        public int CustomerID { get; set; }

        public int ProductID { get; set; }

        public DateTime AddedDate { get; set; }

        public Customer Customer { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}