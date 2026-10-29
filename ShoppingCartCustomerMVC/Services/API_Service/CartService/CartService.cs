using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.CartService
{
    public partial class CartService
    {
        private readonly ShoppingCartDbContext _context;

        public CartService(ShoppingCartDbContext context)
        {
            _context = context;
        }
    }
}