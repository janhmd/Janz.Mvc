using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Kabinet: jurnalda oxuduqlarım
    public class ProfileJournalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfilePageVM model)
        {
            return View(model);
        }
    }
}
