namespace Janz.Mvc.ViewModels.Products
{
    public class HomeIndexVM
    {
        public Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM Reviews { get; set; } = new Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM();
        public List<ProductUIVM> Coffee { get; set; } = new List<ProductUIVM>();
        public List<ProductUIVM> Sets { get; set; } = new List<ProductUIVM>();
    }
}
