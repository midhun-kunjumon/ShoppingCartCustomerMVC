namespace ShoppingCartCustomerMVC.Models.ViewModels.Customer
{
    public class RegisterViewModel
    {
        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? City { get; set; }

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}