using Janz.Mvc.ViewModels.Categories;
using Janz.Mvc.ViewModels.Tags;
using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Products
{
    // Admin: məhsul əlavə etmək və dəyişmək üçün forma (API-yə multipart/form-data kimi gedir)
    public class ProductFormVM
    {
        public int Id { get; set; }

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

        // Yeni əsas şəkil (yaradanda mütləqdir, dəyişəndə istəyə bağlı)
        public IFormFile? Image { get; set; }

        // Qalereyaya əlavə olunan şəkillər
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();

        // Qalereyadan silinəcək köhnə şəkillər
        public List<string> RemoveImages { get; set; } = new List<string>();

        // Səhifədə göstərmək üçün (formadan gəlmir, API-dən doldurulur)
        public string ImagePath { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public List<CategoryUIVM> Categories { get; set; } = new List<CategoryUIVM>();
        public List<TagUIVM> Tags { get; set; } = new List<TagUIVM>();

        public bool IsNew => Id == 0;
    }
}
