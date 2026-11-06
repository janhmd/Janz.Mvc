using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Dashboard;

namespace Janz.Mvc.ViewComponents
{
    public class AdminDashSideViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DashboardVM model)
        {
            return View(model);
        }
    }
}
