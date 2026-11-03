namespace Janz.Mvc.ViewModels.Journal
{
    // Məqalə mətninin bir hissəsi: Heading, Text (abzaslar) və ya Photo
    public class ArticleBlockVM
    {
        public string Kind { get; set; } = "Text";
        public string? Heading { get; set; }
        public List<string> Paragraphs { get; set; } = new List<string>();
        public string? Image { get; set; }
        public string? Caption { get; set; }
    }
}
