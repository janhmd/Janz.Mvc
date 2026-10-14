using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewModels.Cart
{
    public class CartPageVM
    {
        public CartUIVM Cart { get; set; } = new CartUIVM();
        public List<ProductUIVM> Suggestions { get; set; } = new List<ProductUIVM>();
    }
}
