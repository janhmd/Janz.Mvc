using Janz.Mvc.ViewModels.Tags;

namespace Janz.Mvc.ViewModels.Products
{
    // Admin panel: məhsul (api/admin/products). Saytdakı ProductUIVM-in admin tərəfdəki qarşılığıdır
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public decimal? OldPrice { get; set; }
        public string? Badge { get; set; }
        public bool IsVisible { get; set; } = true;
        public string? Roast { get; set; }
        public string? Region { get; set; }
        public string? Processing { get; set; }
        public string? Caffeine { get; set; }
        public string? Brewing { get; set; }
        public string? Storage { get; set; }
        public List<string> BrewMethods { get; set; } = new List<string>();
        public string? Use { get; set; }
        public string? Taste { get; set; }
        public List<string> Notes { get; set; } = new List<string>();
        public int? Density { get; set; }
        public int? Acidity { get; set; }
        public string? TasteGroup { get; set; }
        public decimal? Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductVariantVM> Variants { get; set; } = new List<ProductVariantVM>();


        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public List<TagVM> Tags { get; set; } = new List<TagVM>();
    }
}
