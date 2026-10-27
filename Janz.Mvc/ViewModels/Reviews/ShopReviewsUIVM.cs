namespace Janz.Mvc.ViewModels.Reviews
{
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
}
