namespace Janz.Mvc.ViewModels.Community
{
    public class AdminCommunityCommentVM
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public string Text { get; set; } = string.Empty;
        public string PostText { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
