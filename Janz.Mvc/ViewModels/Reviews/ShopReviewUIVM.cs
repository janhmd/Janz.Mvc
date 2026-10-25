namespace Janz.Mvc.ViewModels.Reviews
{
    public class ShopReviewUIVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorLevel { get; set; } = string.Empty;
        public int ShopRating { get; set; }
        public string? ShopText { get; set; }
        public int DeliveryRating { get; set; }
        public string? DeliveryText { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // api/reviews: Reviews səhifəsi və ana səhifə
    public class ShopReviewsUIVM
    {
        public int Total { get; set; }
        public decimal? ShopAverage { get; set; }
        public decimal? DeliveryAverage { get; set; }
        public List<int> ShopCounts { get; set; } = new List<int>();
        public List<int> DeliveryCounts { get; set; } = new List<int>();
        public List<ShopReviewUIVM> Items { get; set; } = new List<ShopReviewUIVM>();

        // Səhifələmə (yalnız Reviews səhifəsində)
        public int Page { get; set; } = 1;
        public bool HasMore { get; set; }
    }

    // api/reviews/order/{id}: sifariş səhifəsindəki forma
    public class OrderReviewUIVM
    {
        public bool CanReview { get; set; }
        public ShopReviewUIVM? Review { get; set; }
    }
}
