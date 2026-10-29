using ShoppingCartCustomerMVC.Models.Entities;

namespace ShoppingCartCustomerMVC.Services.API_Service.WishlistService
{
    public partial class WishlistService
    {
        public bool MergeWishlist(int customerId, List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return true;
            }

            foreach (var productId in productIds)
            {
                var product = _context.Products
                    .FirstOrDefault(p => p.ProductID == productId);

                if (product == null) continue;

                var existingWishlist = _context.Wishlists
                    .FirstOrDefault(w =>
                        w.CustomerID == customerId &&
                        w.ProductID == productId);

                if (existingWishlist == null)
                {
                    var wishlistItem = new Wishlist
                    {
                        CustomerID = customerId,
                        ProductID = productId,
                        AddedDate = DateTime.Now
                    };

                    _context.Wishlists.Add(wishlistItem);
                }
            }

            _context.SaveChanges();
            return true;
        }
    }
}
