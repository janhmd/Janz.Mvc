namespace Janz.Mvc.ViewModels.Reviews
{
    // api/reviews/order/{id}: sifariş səhifəsindəki forma
    public class OrderReviewUIVM
    {
        public bool CanReview { get; set; }
        public ShopReviewUIVM? Review { get; set; }
    }
}
