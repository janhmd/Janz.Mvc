using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Ana səhifə: həftənin qəhvələri
    public class HomeWeekViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomeIndexVM model)
        {
            return View(model);
        }
    }
}
