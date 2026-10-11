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
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public List<TagUIVM> Tags { get; set; } = new List<TagUIVM>();
    }
}
