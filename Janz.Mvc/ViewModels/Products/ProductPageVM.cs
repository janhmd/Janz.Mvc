namespace Janz.Mvc.ViewModels.Products
{
    public class ProductPageVM
    {
        public ProductUIVM Product { get; set; } = null!;
        public List<ProductUIVM> Related { get; set; } = new List<ProductUIVM>();
    }
}
