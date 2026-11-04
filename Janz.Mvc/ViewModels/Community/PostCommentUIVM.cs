namespace Janz.Mvc.ViewModels.Community
{
    public class PostCommentUIVM
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int? ParentId { get; set; }
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public string Text { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Score { get; set; }
        public int MyVote { get; set; }
        public bool IsMine { get; set; }
        public List<PostCommentUIVM> Replies { get; set; } = new List<PostCommentUIVM>();
    }
}
