namespace Janz.Mvc.ViewModels.Journal
{
    public class ArticleCommentUIVM
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public string? StaffRole { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public bool IsMine { get; set; }
        public int MyVote { get; set; }
    }
}
