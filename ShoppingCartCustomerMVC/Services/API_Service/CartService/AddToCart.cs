using Microsoft.EntityFrameworkCore;
using ShoppingCartCustomerMVC.Models.Entities;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        public bool AddToCart(int customerId, int productId, int quantity)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                return false;
            }

            var cart = _context.Cart
                .FirstOrDefault(c => c.CustomerID == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerID = customerId
                };
                _context.Cart.Add(cart);
                _context.SaveChanges();
            }

            var existingCartItem = _context.CartItems
                .FirstOrDefault(ci =>
                    ci.CartID == cart.CartID &&
                    ci.ProductID == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            return true;
        }
    }
}