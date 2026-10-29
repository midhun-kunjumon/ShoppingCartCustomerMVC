using ShoppingCartCustomerMVC.Models.Entities;

namespace ShoppingCartCustomerMVC.Services.API_Service.WishlistService
{
    public partial class WishlistService
    {
        public bool AddToWishlist(int customerId, int productId)
        {
            var productExists = _context.Products
                .Any(p => p.ProductID == productId);

            if (!productExists)
            {
                return false;
            }

            var existingWishlistItem = _context.Wishlists
                .FirstOrDefault(w =>
                    w.CustomerID == customerId &&
                    w.ProductID == productId);

            if (existingWishlistItem != null)
            {
                return false;
            }

            var wishlistItem = new Wishlist
            {
                CustomerID = customerId,
                ProductID = productId,
                AddedDate = DateTime.Now
            };

            _context.Wishlists.Add(wishlistItem);

            _context.SaveChanges();

            return true;
        }
    }
}