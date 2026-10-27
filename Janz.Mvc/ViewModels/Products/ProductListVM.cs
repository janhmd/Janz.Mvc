using Janz.Mvc.ViewModels.Categories;

namespace Janz.Mvc.ViewModels.Products
{
    // Admin: məhsulların siyahısı, axtarış və kateqoriya filtri
    public class ProductListVM
    {
        public List<ProductVM> Products { get; set; } = new List<ProductVM>();
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public int Total { get; set; }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public bool LowStock { get; set; }
    }
}
