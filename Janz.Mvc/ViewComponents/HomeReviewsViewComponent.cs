using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Ana səhifə: rəylər
    public class HomeReviewsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomeIndexVM model)
        {
            return View(model);
        }
    }
}
