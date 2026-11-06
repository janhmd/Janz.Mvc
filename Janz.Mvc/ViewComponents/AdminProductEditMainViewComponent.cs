using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class AdminProductEditMainViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductEditVM model)
        {
            return View(model);
        }
    }
}
