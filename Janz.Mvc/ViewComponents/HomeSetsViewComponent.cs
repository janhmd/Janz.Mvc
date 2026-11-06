using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Ana səhifə: dəstlər
    public class HomeSetsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomeIndexVM model)
        {
            return View(model);
        }
    }
}
