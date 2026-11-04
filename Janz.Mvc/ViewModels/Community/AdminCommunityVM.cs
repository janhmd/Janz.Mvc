namespace Janz.Mvc.ViewModels.Community
{
    public class AdminCommunityVM
    {
        public string Tab { get; set; } = "posts";
        public List<AdminCommunityPostVM> Posts { get; set; } = new List<AdminCommunityPostVM>();
        public List<AdminCommunityCommentVM> Comments { get; set; } = new List<AdminCommunityCommentVM>();
        public List<AdminCommunityReportVM> Reports { get; set; } = new List<AdminCommunityReportVM>();
    }
}
