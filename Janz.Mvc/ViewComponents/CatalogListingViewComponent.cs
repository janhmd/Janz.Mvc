using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class CatalogListingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<ProductUIVM> model)
        {
            return View(model);
        }
    }
}
