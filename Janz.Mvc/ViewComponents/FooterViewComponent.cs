using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Header;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Saytın footer-i: Help bağlantıları (mətn səhifələri) və əlaqə məlumatı admin paneldən gəlir
    public class FooterViewComponent : ViewComponent
    {
        private readonly SiteContent _siteContent;

        public FooterViewComponent(SiteContent siteContent)
        {
            _siteContent = siteContent;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new FooterVM
            {
                Pages = await _siteContent.PagesAsync(),
                Setting = await _siteContent.SettingAsync()
            });
        }
    }
}
