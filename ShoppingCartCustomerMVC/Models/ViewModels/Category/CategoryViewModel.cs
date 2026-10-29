namespace ShoppingCartCustomerMVC.Models.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}