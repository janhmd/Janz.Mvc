namespace Janz.Mvc.ViewModels.Journal
{
    // api/articles: jurnal kartı
    public class ArticleCardUIVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string Lead { get; set; } = string.Empty;
        public string CoverPath { get; set; } = string.Empty;
        public bool IsLearning { get; set; }
        public string? Series { get; set; }
        public DateTime PublishedAt { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Views { get; set; }
        public int Minutes { get; set; }
        public bool IsLiked { get; set; }
        public bool IsSaved { get; set; }
    }
}
