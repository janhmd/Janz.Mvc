namespace Janz.Mvc.ViewModels.Community
{
    public class CommunityPageVM
    {
        public List<PostUIVM> Posts { get; set; } = new List<PostUIVM>();
        public string Tab { get; set; } = "best";
        public string? Tag { get; set; }
        public string? Search { get; set; }
        public string Scope { get; set; } = "everywhere";
        public List<string> Tags { get; set; } = new List<string>();
    }
}
