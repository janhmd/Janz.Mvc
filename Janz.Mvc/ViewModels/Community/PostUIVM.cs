namespace Janz.Mvc.ViewModels.Community
{
    public class PostUIVM
    {
        public int Id { get; set; }
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public string Text { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public PostProductUIVM? Product { get; set; }
        public int? Rating { get; set; }
        public string? Brew { get; set; }
        public string? Recipe { get; set; }
        public string? Dose { get; set; }
        public string? Water { get; set; }
        public string? Grind { get; set; }
        public string? Time { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public int Score { get; set; }
        public int MyVote { get; set; }
        public bool IsSaved { get; set; }
        public bool IsMine { get; set; }
        public bool IsFollowing { get; set; }
        public int CommentCount { get; set; }
        public List<PostCommentUIVM> Comments { get; set; } = new List<PostCommentUIVM>();
    }
}
