namespace Janz.Mvc.ViewModels.Cart
{
    // api/coupons/check cavabı: səbətdə yazılan promo kodun endirimi
    public class CouponCheckUIVM
    {
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
    }
}
