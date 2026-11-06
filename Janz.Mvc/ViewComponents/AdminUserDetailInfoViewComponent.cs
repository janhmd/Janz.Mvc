using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Users;

namespace Janz.Mvc.ViewComponents
{
    public class AdminUserDetailInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(UserAdminVM model)
        {
            return View(model);
        }
    }
}
