namespace ShoppingCartCustomerMVC.Models.ViewModels.Customer
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? City { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string? PasswordHash { get; set; }

        public string? AuthProvider { get; set; }

        public string? GoogleId { get; set; }
    }
}