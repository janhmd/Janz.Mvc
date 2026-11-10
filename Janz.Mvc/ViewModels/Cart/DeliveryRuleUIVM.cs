namespace Janz.Mvc.ViewModels.Cart
{
    // api/orders/delivery: çatdırılma haqqı və pulsuz çatdırılma həddi
    public class DeliveryRuleUIVM
    {
        public decimal Fee { get; set; }
        public decimal FreeFrom { get; set; }
    }
}
