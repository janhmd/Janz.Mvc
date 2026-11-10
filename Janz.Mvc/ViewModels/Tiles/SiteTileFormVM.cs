using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Tiles
{
    // Admin: kartın yaradılması və dəyişdirilməsi
    public class SiteTileFormVM
    {
        // API-dəki Domain/Constants/TilePlacements.cs ilə eyni olmalıdır: (açar, göstərilən ad)
        public static readonly (string Key, string Label)[] Placements =
        {
            ("home", "Home page"),
            ("coffee", "Coffee page"),
            ("tea", "Tea page"),
            ("chocolate", "Chocolate and syrups page"),
            ("sets", "Sets page")
        };

        public int Id { get; set; }

        [Required(ErrorMessage = "Choose where the tile is shown.")]
        public string Placement { get; set; } = "home";

        [Required(ErrorMessage = "Enter the title.")]
        [StringLength(40, ErrorMessage = "The title can be at most 40 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the link.")]
        [StringLength(150, ErrorMessage = "The link can be at most 150 characters.")]
        public string Link { get; set; } = string.Empty;

        [Range(0, 999, ErrorMessage = "The order must be between 0 and 999.")]
        public int SortOrder { get; set; }

        public bool IsVisible { get; set; } = true;

        public IFormFile? Image { get; set; }

        // Dəyişdirəndə göstərilir
        public string CurrentImage { get; set; } = string.Empty;
        public bool HasVideo { get; set; }
    }
}
