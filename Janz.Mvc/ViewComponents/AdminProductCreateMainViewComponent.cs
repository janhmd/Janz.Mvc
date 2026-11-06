using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class AdminProductCreateMainViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductCreateVM model)
        {
            return View(model);
        }
    }
}
