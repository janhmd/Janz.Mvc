using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Ana səhifə: yeni məhsullar
    public class HomeNewViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomeIndexVM model)
        {
            return View(model);
        }
    }
}
