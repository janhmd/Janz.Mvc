namespace Janz.Mvc.ViewModels.Cart
{
    // api/coupons/first-order: hələ sifarişi olmayan müştəriyə göstərilən kupon
    public class FirstOrderCouponUIVM
    {
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Value { get; set; }

        public string Discount => Type == "Percent" ? $"{Value:0.##}%" : $"₼{Value:0.00}";
    }
}
