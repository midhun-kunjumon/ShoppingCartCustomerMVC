namespace ShoppingCartCustomerMVC.Services.API_Service.WishlistService
{
    public partial class WishlistService
    {
        public bool RemoveFromWishlist(int customerId, int wishlistId)
        {
            var wishlistItem = _context.Wishlists
                .FirstOrDefault(w =>
                    w.WishlistID == wishlistId &&
                    w.CustomerID == customerId);

            if (wishlistItem == null)
            {
                return false;
            }

            _context.Wishlists.Remove(wishlistItem);

            _context.SaveChanges();

            return true;
        }
    }
}