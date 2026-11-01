namespace Janz.Mvc.ViewModels.Cart
{
    // api/discount-levels/me: müştərinin endirim səviyyəsi
    public class DiscountLevelUIVM
    {
        public string? Name { get; set; }
        public int Percent { get; set; }
        public decimal Spent { get; set; }
        public string? NextName { get; set; }
        public decimal? NextMinSpent { get; set; }
    }
}
