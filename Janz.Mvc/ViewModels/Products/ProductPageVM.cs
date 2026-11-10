namespace Janz.Mvc.ViewModels.Products
{
    public class ProductPageVM
    {
        public ProductUIVM Product { get; set; } = null!;
        public List<ProductUIVM> Related { get; set; } = new List<ProductUIVM>();
        public Janz.Mvc.ViewModels.Reviews.ProductReviewsUIVM Reviews { get; set; } = new Janz.Mvc.ViewModels.Reviews.ProductReviewsUIVM();

        // Community-də bu məhsulla paylaşılan postlar: Comments bölməsində rəylərin yanında görünür
        public List<Janz.Mvc.ViewModels.Community.PostUIVM> Posts { get; set; } = new List<Janz.Mvc.ViewModels.Community.PostUIVM>();

        // Daxil olmuş, hələ sifarişi olmayan müştəriyə ilk sifariş kuponu
        public Janz.Mvc.ViewModels.Cart.FirstOrderCouponUIVM? Coupon { get; set; }

        // Məhsul seçilmişlərdədirsə ürək dolu görünür
        public bool InWishlist { get; set; }
    }
}
