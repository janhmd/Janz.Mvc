using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Pages
{
    public class PageEditVM
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }

        [Required(ErrorMessage = "Enter the title.")]
        [StringLength(80, ErrorMessage = "The title can be at most 80 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the text.")]
        public string Body { get; set; } = string.Empty;

        public bool InTerms { get; set; }
        public bool InFooter { get; set; }
    }
}
