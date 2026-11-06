using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class AdminProductsTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductListVM model)
        {
            return View(model);
        }
    }
}
