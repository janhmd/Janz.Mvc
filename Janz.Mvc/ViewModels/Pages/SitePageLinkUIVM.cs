namespace Janz.Mvc.ViewModels.Pages
{
    public class SitePageLinkUIVM
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool InTerms { get; set; }
        public bool InFooter { get; set; }
    }
}
