namespace Janz.Mvc.ViewModels.Community
{
    public class CommunityPersonUIVM
    {
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public bool IsFollowing { get; set; }
        public bool IsMe { get; set; }
    }
}
