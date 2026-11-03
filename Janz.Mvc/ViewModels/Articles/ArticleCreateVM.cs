using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Articles
{
    // Admin: məqalə forması. Mətndə boş sətir yeni abzasdır, "## " başlıqdır, "[photo 2: caption]" 2-ci şəkli qoyur
    public class ArticleCreateVM
    {
        [Required(ErrorMessage = "Enter the title.")]
        [StringLength(200, ErrorMessage = "The title can be at most 200 characters.")]
        public string? Title { get; set; }

        [RegularExpression("^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "The slug can have only small latin letters, digits and dashes.")]
        [StringLength(200, ErrorMessage = "The slug can be at most 200 characters.")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Choose a topic.")]
        public string? Topic { get; set; }

        [Required(ErrorMessage = "Enter the lead.")]
        [StringLength(300, ErrorMessage = "The lead can be at most 300 characters.")]
        public string? Lead { get; set; }

        [Required(ErrorMessage = "Write the article text.")]
        public string? Body { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        [StringLength(100, ErrorMessage = "The author name can be at most 100 characters.")]
        public string? AuthorName { get; set; }

        [StringLength(100, ErrorMessage = "The author role can be at most 100 characters.")]
        public string? AuthorRole { get; set; }

        public bool IsLearning { get; set; }

        [StringLength(100, ErrorMessage = "The series can be at most 100 characters.")]
        public string? Series { get; set; }

        public bool IsPublished { get; set; }

        public IFormFile? Cover { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
