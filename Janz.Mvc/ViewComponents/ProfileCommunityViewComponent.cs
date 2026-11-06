using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Kabinet: Community profili (bio, "How I cook", paylaşımlar)
    public class ProfileCommunityViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfilePageVM model)
        {
            return View(model);
        }
    }
}
