using Janz.Mvc.ViewModels.Tags;

namespace Janz.Mvc.ViewModels.Products
{
    public class ProductUIVM
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

        // Endirim faizi: köhnə qiymət yalnız indikindən böyük olanda
        public int DiscountPercent => OldPrice is decimal old && old > Price ? (int)Math.Round((old - Price) / old * 100) : 0;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public List<TagUIVM> Tags { get; set; } = new List<TagUIVM>();
    }
}
