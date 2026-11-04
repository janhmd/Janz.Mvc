namespace Janz.Mvc.ViewModels.Community
{
    public class CommunityProfileUIVM
    {
        public AuthorUIVM Author { get; set; } = new AuthorUIVM();
        public string? Bio { get; set; }
        public List<string> CookMethods { get; set; } = new List<string>();
        public int Followers { get; set; }
        public int Following { get; set; }
        public bool IsMe { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsBlocked { get; set; }
        public CommunityStatsUIVM Stats { get; set; } = new CommunityStatsUIVM();
        public List<PostUIVM> Posts { get; set; } = new List<PostUIVM>();
    }
}
