using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class ProductDetailBuyViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductPageVM model)
        {
            return View(model);
        }
    }
}
