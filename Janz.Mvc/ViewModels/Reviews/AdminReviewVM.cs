namespace Janz.Mvc.ViewModels.Reviews
{
    public class AdminReviewVM
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? OrderId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int? DeliveryRating { get; set; }
        public string? Text { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminReviewListVM
    {
        public List<AdminReviewVM> Reviews { get; set; } = new List<AdminReviewVM>();
        public string? Type { get; set; }
        public bool? Hidden { get; set; }
        public string? Search { get; set; }
    }
}
