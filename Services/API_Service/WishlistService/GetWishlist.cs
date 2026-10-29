using Microsoft.EntityFrameworkCore;
using ShoppingCartCustomerMVC.Models.ViewModels.Wishlist;

namespace ShoppingCartCustomerMVC.Services.API_Service.WishlistService
{
    public partial class WishlistService
    {
        public List<WishlistViewModel> GetWishlist(int customerId)
        {
            return _context.Wishlists
                .AsNoTracking()
                .Where(w => w.CustomerID == customerId)
                .Select(w => new WishlistViewModel
                {
                    WishlistID = w.WishlistID,
                    ProductID = w.ProductID,
                    ProductName = w.Product.ProductName,
                    Price = w.Product.Price,
                    ImageUrl = w.Product.ImageUrl,
                    AddedDate = w.AddedDate
                })
                .OrderByDescending(w => w.AddedDate)
                .ToList();
        }
    }
}