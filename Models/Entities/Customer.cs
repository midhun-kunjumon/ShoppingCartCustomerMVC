namespace ShoppingCartCustomerMVC.Models.Entities
{
    public class Customer
    {
        public int CustomerID { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? City { get; set; }

        public Cart? Cart { get; set; }
    }
}