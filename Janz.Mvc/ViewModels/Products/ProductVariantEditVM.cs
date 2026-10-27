namespace Janz.Mvc.ViewModels.Products
{
    // Admin formasında bir variant sətri (Variants[0].Format, Variants[0].NewImages ...)
    public class ProductVariantEditVM
    {
        public int? Id { get; set; }
        public string? Format { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int? Stock { get; set; }

        public List<string> KeepImages { get; set; } = new List<string>();
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        // Səhifədə göstərmək üçün: variantın hazırkı şəkilləri
        public List<string> Images { get; set; } = new List<string>();
    }
}
