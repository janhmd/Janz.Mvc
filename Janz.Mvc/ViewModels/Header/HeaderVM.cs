using Janz.Mvc.ViewModels.Cart;

namespace Janz.Mvc.ViewModels.Header
{
    public class HeaderVM
    {
        public bool SignedIn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CartCount { get; set; }
        public int ReviewCount { get; set; }

        // Header-dəki səbət panelində göstərilən məhsullar
        public CartUIVM Cart { get; set; } = new CartUIVM();
    }
}
