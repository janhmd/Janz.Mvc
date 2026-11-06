using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class AdminDiscountLevelsFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.DiscountLevels.DiscountLevelListVM model)
        {
            return View(model);
        }
    }
}
