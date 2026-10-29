namespace ShoppingCartCustomerMVC.Models.ViewModels.Auth
{
    public class RefreshTokenViewModel
    {
        public int RefreshTokenID { get; set; }

        public int CustomerID { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}