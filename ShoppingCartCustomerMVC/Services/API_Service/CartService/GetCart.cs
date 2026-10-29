using Microsoft.EntityFrameworkCore;
using ShoppingCartCustomerMVC.Models.ViewModels.Cart;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        public List<CartViewModel> GetCart(int customerId)
        {
            return _context.CartItems
                .AsNoTracking()
                .Where(ci => ci.Cart.CustomerID == customerId)
                .Select(ci => new CartViewModel
                {
                    CartItemID = ci.CartItemID,
                    CartID = ci.CartID,
                    ProductID = ci.ProductID,
                    ProductName = ci.Product.ProductName,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Total = ci.Quantity * ci.Product.Price,
                    ImageUrl = ci.Product.ImageUrl
                })
                .OrderBy(ci => ci.CartItemID)
                .ToList();
        }
    }
}