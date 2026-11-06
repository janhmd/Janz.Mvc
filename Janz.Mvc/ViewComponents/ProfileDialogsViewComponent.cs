using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Kabinet: pəncərələr (məlumat, ünvan, balans, "How I cook", abunəçilər, qara siyahıdan çıxarma)
    public class ProfileDialogsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfilePageVM model)
        {
            return View(model);
        }
    }
}
