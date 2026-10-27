using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Categories
{
    // Admin: kateqoriyanı dəyişmək. Slug boş qalsa API addan düzəldir
    public class CategoryEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the name.")]
        [StringLength(100, ErrorMessage = "The name can be at most 100 characters.")]
        public string? Name { get; set; }

        [StringLength(100, ErrorMessage = "The slug can be at most 100 characters.")]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "The slug can have only small latin letters, numbers and hyphens.")]
        public string? Slug { get; set; }
    }
}
