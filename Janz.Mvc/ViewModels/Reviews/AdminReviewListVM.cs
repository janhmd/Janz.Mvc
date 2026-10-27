namespace Janz.Mvc.ViewModels.Reviews
{
    public class AdminReviewListVM
    {
        public List<AdminReviewVM> Reviews { get; set; } = new List<AdminReviewVM>();
        public string? Type { get; set; }
        public bool? Hidden { get; set; }
        public string? Search { get; set; }
    }
}
