namespace Janz.Mvc.ViewModels.Reviews
{
    // api/reviews: Reviews səhifəsi və ana səhifə
    public class ShopReviewsUIVM
    {
        public int Total { get; set; }
        public int Matching { get; set; }
        public decimal? ShopAverage { get; set; }
        public decimal? DeliveryAverage { get; set; }
        public List<int> ShopCounts { get; set; } = new List<int>();
        public List<int> DeliveryCounts { get; set; } = new List<int>();
        public List<ShopReviewUIVM> Items { get; set; } = new List<ShopReviewUIVM>();

        // Səhifələmə (yalnız Reviews səhifəsində)
        public int Page { get; set; } = 1;
        public bool HasMore { get; set; }

        // Süzgəclər (Reviews səhifəsi): ulduzlar və axtarış mətni
        public List<int> ShopStars { get; set; } = new List<int>();
        public List<int> DeliveryStars { get; set; } = new List<int>();
        public string? Search { get; set; }

        // "Show more" və səhifələmə üçün cari süzgəclərin sorğu mətni
        public string FilterQuery(int page)
        {
            var parts = new List<string> { $"page={page}" };
            if (ShopStars.Count > 0) parts.Add("shop=" + string.Join(",", ShopStars));
            if (DeliveryStars.Count > 0) parts.Add("delivery=" + string.Join(",", DeliveryStars));
            if (!string.IsNullOrWhiteSpace(Search)) parts.Add("q=" + Uri.EscapeDataString(Search.Trim()));
            return string.Join("&", parts);
        }
    }
}
