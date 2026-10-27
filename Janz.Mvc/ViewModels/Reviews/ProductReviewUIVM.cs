namespace Janz.Mvc.ViewModels.Reviews
{
    public class ProductReviewUIVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsMine { get; set; }
    }
}
