namespace ShoppingCartCustomerMVC.Models.ViewModels.Category
{
    public class SubCategoryViewModel
    {
        public int SubCategoryID { get; set; }

        public string SubCategoryName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string? ImageUrl { get; set; }
    }
}