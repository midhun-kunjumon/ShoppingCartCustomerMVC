using ShoppingCartCustomerMVC.Data;

namespace ShoppingCartCustomerMVC.Services.API_Service.WishlistService
{
    public partial class WishlistService
    {
        private readonly ShoppingCartDbContext _context;

        public WishlistService(ShoppingCartDbContext context)
        {
            _context = context;
        }
    }
}