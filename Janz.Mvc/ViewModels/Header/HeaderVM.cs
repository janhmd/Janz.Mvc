using Janz.Mvc.ViewModels.Cart;

namespace Janz.Mvc.ViewModels.Header
{
    public class HeaderVM
    {
        public bool SignedIn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CartCount { get; set; }
        public int ReviewCount { get; set; }
        public int WishCount { get; set; }

        // Terms menyusundakı mətn səhifələri (admin paneldən idarə olunur)
        public List<Janz.Mvc.ViewModels.Pages.SitePageLinkUIVM> Pages { get; set; } = new List<Janz.Mvc.ViewModels.Pages.SitePageLinkUIVM>();

        // Zəngdəki bildirişlər
        public Janz.Mvc.ViewModels.Notifications.NotificationListUIVM Notifications { get; set; } = new Janz.Mvc.ViewModels.Notifications.NotificationListUIVM();

        // Header-dəki səbət panelində göstərilən məhsullar
        public CartUIVM Cart { get; set; } = new CartUIVM();
    }
}
