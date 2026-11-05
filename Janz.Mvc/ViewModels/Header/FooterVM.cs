using Janz.Mvc.ViewModels.Pages;
using Janz.Mvc.ViewModels.Settings;

namespace Janz.Mvc.ViewModels.Header
{
    public class FooterVM
    {
        public List<SitePageLinkUIVM> Pages { get; set; } = new List<SitePageLinkUIVM>();
        public SiteSettingUIVM Setting { get; set; } = new SiteSettingUIVM();
    }
}
