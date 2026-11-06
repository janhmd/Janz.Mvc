using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Ana səhifə: giriş bölməsi
    public class HomeHeroViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomeIndexVM model)
        {
            return View(model);
        }
    }
}
