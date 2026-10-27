namespace Janz.Mvc.ViewModels.Reviews
{
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
