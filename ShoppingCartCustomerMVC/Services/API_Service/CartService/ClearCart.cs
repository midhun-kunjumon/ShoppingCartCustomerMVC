using Microsoft.EntityFrameworkCore;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        public bool ClearCart(int customerId)
        {
            var cartItems = _context.CartItems
                .Where(ci => ci.Cart.CustomerID == customerId)
                .ToList();

            if (!cartItems.Any())
            {
                return false;
            }

            _context.CartItems.RemoveRange(cartItems);

            _context.SaveChanges();

            return true;
        }
    }
}
