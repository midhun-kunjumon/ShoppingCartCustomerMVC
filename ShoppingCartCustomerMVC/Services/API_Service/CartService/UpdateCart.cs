using Microsoft.EntityFrameworkCore;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        public bool UpdateCart(int customerId, int cartItemId, int quantity)
        {
            var cartItem = _context.CartItems
                .FirstOrDefault(ci =>
                    ci.CartItemID == cartItemId &&
                    ci.Cart.CustomerID == customerId);

            if (cartItem == null)
            {
                return false;
            }

            cartItem.Quantity = quantity;

            _context.SaveChanges();

            return true;
        }
    }
}