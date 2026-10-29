namespace ShoppingCartCustomerMVC.Models.Entities
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int CategoryID { get; set; }

        public int SubCategoryID { get; set; }

        public string? Description { get; set; }

        public string? Brand { get; set; }

        public string? ImageUrl { get; set; }
    }
}