using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Kabinet: müştərinin əlaqə formundan yazdıqları və support cavabları
    public class ProfileMessagesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfilePageVM model)
        {
            return View(model);
        }
    }
}
