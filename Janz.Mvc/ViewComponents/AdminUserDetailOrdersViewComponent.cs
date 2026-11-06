using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Users;

namespace Janz.Mvc.ViewComponents
{
    public class AdminUserDetailOrdersViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(UserAdminVM model)
        {
            return View(model);
        }
    }
}
