using Microsoft.EntityFrameworkCore;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        public bool RemoveFromCart(int customerId, int cartItemId)
        {
            var cartItem = _context.CartItems
                .FirstOrDefault(ci =>
                    ci.CartItemID == cartItemId &&
                    ci.Cart.CustomerID == customerId);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);

            _context.SaveChanges();

            return true;
        }
    }
}