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

    // api/products/{id}/reviews: məhsul səhifəsindəki "Comments" bölməsi
    public class ProductReviewsUIVM
    {
        public decimal? Average { get; set; }
        public int Count { get; set; }
        public List<ProductReviewUIVM> Items { get; set; } = new List<ProductReviewUIVM>();
        public bool CanReview { get; set; }
        public ProductReviewUIVM? MyReview { get; set; }
    }
}
