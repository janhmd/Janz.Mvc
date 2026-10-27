using Janz.Mvc.ViewModels.Categories;
using Janz.Mvc.ViewModels.Tags;
using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Products
{
    // Admin: yeni məhsul formu (API-yə multipart/form-data kimi gedir)
    public class ProductCreateVM
    {
        [Required(ErrorMessage = "Enter the name.")]
        [StringLength(150, ErrorMessage = "The name can be at most 150 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Enter the description.")]
        [StringLength(500, ErrorMessage = "The description can be at most 500 characters.")]
        public string? Description { get; set; }

        [Range(0.01, 100000, ErrorMessage = "The price must be between 0.01 and 100 000.")]
        public decimal Price { get; set; }

        [Range(0, 100000, ErrorMessage = "The stock must be between 0 and 100 000.")]
        public int Stock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Choose a category.")]
        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();

        [Range(0.01, 100000, ErrorMessage = "The old price must be between 0.01 and 100 000.")]
        public decimal? OldPrice { get; set; }

        public string? Badge { get; set; }

        public bool IsVisible { get; set; } = true;

        [StringLength(50, ErrorMessage = "The roast level can be at most 50 characters.")]
        public string? Roast { get; set; }

        [StringLength(50, ErrorMessage = "The region can be at most 50 characters.")]
        public string? Region { get; set; }

        [StringLength(50, ErrorMessage = "The processing can be at most 50 characters.")]
        public string? Processing { get; set; }

        [StringLength(20, ErrorMessage = "The caffeine can be at most 20 characters.")]
        public string? Caffeine { get; set; }

        [StringLength(1000, ErrorMessage = "The brewing text can be at most 1000 characters.")]
        public string? Brewing { get; set; }

        [StringLength(500, ErrorMessage = "The storage text can be at most 500 characters.")]
        public string? Storage { get; set; }

        public List<string> BrewMethods { get; set; } = new List<string>();

        // Kartdakı dad təsviri: "For espresso", "Dense coffee with notes of ...", zolaqlar və Taste filtri
        [StringLength(50, ErrorMessage = "The use can be at most 50 characters.")]
        public string? Use { get; set; }

        [StringLength(60, ErrorMessage = "The taste can be at most 60 characters.")]
        public string? Taste { get; set; }

        [StringLength(150, ErrorMessage = "The notes can be at most 150 characters.")]
        public string? Notes { get; set; }

        [Range(0, 100, ErrorMessage = "The density must be from 0 to 100.")]
        public int? Density { get; set; }

        [Range(0, 100, ErrorMessage = "The acidity must be from 0 to 100.")]
        public int? Acidity { get; set; }

        public string? TasteGroup { get; set; }

        // Variantlar varsa qiymət və stok onlardandır (məhsulun öz qiymət sahələri istifadə olunmur)
        public List<ProductVariantEditVM> Variants { get; set; } = new List<ProductVariantEditVM>();
        public bool HasVariants => Variants.Count > 0;

        // Əsas şəkil (mütləqdir)
        public IFormFile? Image { get; set; }

        // Qalereyaya əlavə olunan şəkillər
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();

        // Səhifədə göstərmək üçün (formadan gəlmir, API-dən doldurulur)
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public List<TagVM> Tags { get; set; } = new List<TagVM>();
    }
}
