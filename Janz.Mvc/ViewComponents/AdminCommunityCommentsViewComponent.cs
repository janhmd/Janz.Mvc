using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Community;

namespace Janz.Mvc.ViewComponents
{
    public class AdminCommunityCommentsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminCommunityVM model)
        {
            return View(model);
        }
    }
}
