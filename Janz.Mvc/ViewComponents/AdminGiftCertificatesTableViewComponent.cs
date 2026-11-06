using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class AdminGiftCertificatesTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.GiftCertificates.GiftCertificateListVM model)
        {
            return View(model);
        }
    }
}
