namespace Janz.Mvc.ViewModels.Journal
{
    public class ArticlePageVM
    {
        public ArticleUIVM Article { get; set; } = new ArticleUIVM();
        public List<ArticleBlockVM> Blocks { get; set; } = new List<ArticleBlockVM>();
        public List<ArticleCommentUIVM> Comments { get; set; } = new List<ArticleCommentUIVM>();
    }
}
