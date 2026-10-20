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
    }
}
