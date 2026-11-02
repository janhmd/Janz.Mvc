using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewModels.Cart
{
    public class CartPageVM
    {
        public CartUIVM Cart { get; set; } = new CartUIVM();
        public List<ProductUIVM> Suggestions { get; set; } = new List<ProductUIVM>();

        // Sifariş üçün çatdırılma ünvanları (giriş edilməyibsə boşdur)
        public List<AddressUIVM> Addresses { get; set; } = new List<AddressUIVM>();

        // Sifariş formunda seçili ünvan (yeni ünvan əlavə olunanda o seçilir)
        public int? SelectedAddressId { get; set; }

        // Alıcı bölməsi: hesabın adı və emaili
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Promo kod: yazılan kod, tətbiq olunubsa endirimi, olmayıbsa xəta
        public string? Promo { get; set; }
        public CouponCheckUIVM? Coupon { get; set; }
        public string? PromoError { get; set; }

        // Endirim səviyyəsi: kupon və səviyyə toplanmır, böyüyü tətbiq olunur (API də belə hesablayır)
        public DiscountLevelUIVM? Level { get; set; }
        public decimal LevelDiscount => Level is null ? 0 : Math.Round(Cart.Total * Level.Percent / 100, 2);
        public bool UsesCoupon => Coupon is not null && Coupon.Discount > LevelDiscount;
        public bool UsesLevel => !UsesCoupon && LevelDiscount > 0;

        // Daxili hesab: sifarişdə istifadə olunarsa ödəniləcək məbləğdən çıxılır (çatmazsa qalanı kartla və ya çatdırılmada)
        public decimal Balance { get; set; }
        public decimal BalancePart => Math.Min(Balance, PayTotal);

        public decimal PayTotal => Cart.Total - (UsesCoupon ? Coupon!.Discount : UsesLevel ? LevelDiscount : 0);
    }
}
