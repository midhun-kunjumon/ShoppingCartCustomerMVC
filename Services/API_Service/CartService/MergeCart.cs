using Microsoft.EntityFrameworkCore;
using ShoppingCartCustomerMVC.Models.Entities;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public class AnonymousCartItemDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public partial class CartService
    {
        public bool MergeCart(int customerId, List<AnonymousCartItemDto> anonymousItems)
        {
            if (anonymousItems == null || !anonymousItems.Any())
            {
                return true;
            }

            var validAnonymousItems = anonymousItems
                .Where(i => i != null && i.ProductID > 0 && i.Quantity > 0)
                .GroupBy(i => i.ProductID)
                .Select(g => new AnonymousCartItemDto
                {
                    ProductID = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            if (!validAnonymousItems.Any())
            {
                return true;
            }

            var cart = _context.Cart
                .Include(c => c.CartItems)
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

            foreach (var item in validAnonymousItems)
            {
                int productId = item.ProductID;
                int quantity = item.Quantity;

                var product = _context.Products
                    .FirstOrDefault(p => p.ProductID == productId);

                if (product == null) continue;

                var existingCartItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductID == productId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        CartID = cart.CartID,
                        ProductID = productId,
                        Quantity = quantity
                    };

                    cart.CartItems.Add(newCartItem);
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                _context.ChangeTracker.Clear();

                var reloadedCart = _context.Cart
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.CustomerID == customerId);

                if (reloadedCart != null)
                {
                    foreach (var item in validAnonymousItems)
                    {
                        var existingItem = reloadedCart.CartItems
                            .FirstOrDefault(ci => ci.ProductID == item.ProductID);

                        if (existingItem != null)
                        {
                            existingItem.Quantity += item.Quantity;
                        }
                        else
                        {
                            reloadedCart.CartItems.Add(new CartItem
                            {
                                CartID = reloadedCart.CartID,
                                ProductID = item.ProductID,
                                Quantity = item.Quantity
                            });
                        }
                    }
                    _context.SaveChanges();
                }
            }

            return true;
        }
    }
}
