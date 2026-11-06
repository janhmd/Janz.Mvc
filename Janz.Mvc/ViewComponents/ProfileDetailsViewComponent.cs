using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Kabinet: şəxsi məlumatlar və ünvanlar
    public class ProfileDetailsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfilePageVM model)
        {
            return View(model);
        }
    }
}
