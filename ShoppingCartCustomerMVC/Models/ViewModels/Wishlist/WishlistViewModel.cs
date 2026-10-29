namespace ShoppingCartCustomerMVC.Models.ViewModels.Wishlist
{
    public class WishlistViewModel
    {
        public int WishlistID { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime AddedDate { get; set; }
    }
}