namespace Janz.Mvc.ViewModels.Community
{
    public class AdminCommunityPostVM
    {
        public int Id { get; set; }
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public string Text { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public int Reports { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
