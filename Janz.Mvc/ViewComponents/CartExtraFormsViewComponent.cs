using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class CartExtraFormsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Cart.CartPageVM model)
        {
            return View(model);
        }
    }
}
