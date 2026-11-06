using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewComponents
{
    public class AdminProductDetailVariantsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductVM model)
        {
            return View(model);
        }
    }
}
