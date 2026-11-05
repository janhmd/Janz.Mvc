namespace Janz.Mvc.ViewModels.Pages
{
    public class SitePageUIVM
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
