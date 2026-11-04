namespace Janz.Mvc.ViewModels.Products
{
    public class HomeIndexVM
    {
        public Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM Reviews { get; set; } = new Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM();
        public List<ProductUIVM> Coffee { get; set; } = new List<ProductUIVM>();
        public List<ProductUIVM> Sets { get; set; } = new List<ProductUIVM>();

        // Jurnalın ən son əlavə olunan məqalələri (ana səhifə slayderi)
        public List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM> LatestArticles { get; set; } = new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>();

        // Daxil olmuş, hələ sifarişi olmayan müştəriyə ilk sifariş kuponu
        public Janz.Mvc.ViewModels.Cart.FirstOrderCouponUIVM? Coupon { get; set; }
    }
}
