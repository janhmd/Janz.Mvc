using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class ProductDetailExtrasViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductPageVM model)
        {
            return View(model);
        }
    }
}
