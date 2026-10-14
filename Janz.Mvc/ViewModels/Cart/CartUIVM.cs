namespace Janz.Mvc.ViewModels.Cart
{
    public class CartUIVM
    {
        public List<CartItemUIVM> Items { get; set; } = new List<CartItemUIVM>();
        public int Count { get; set; }
        public decimal Total { get; set; }
    }
}
