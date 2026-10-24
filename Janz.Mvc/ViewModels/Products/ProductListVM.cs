using Janz.Mvc.ViewModels.Categories;

namespace Janz.Mvc.ViewModels.Products
{
    // Admin: məhsulların siyahısı, axtarış və kateqoriya filtri
    public class ProductListVM
    {
        public List<ProductUIVM> Products { get; set; } = new List<ProductUIVM>();
        public List<CategoryUIVM> Categories { get; set; } = new List<CategoryUIVM>();
        public int Total { get; set; }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public bool LowStock { get; set; }
    }
}
