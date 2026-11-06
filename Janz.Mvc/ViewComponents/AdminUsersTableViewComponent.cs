using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Users;

namespace Janz.Mvc.ViewComponents
{
    public class AdminUsersTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(UserListVM model)
        {
            return View(model);
        }
    }
}
