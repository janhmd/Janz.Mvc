using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class AdminCategoriesTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Janz.Mvc.ViewModels.Categories.CategoryVM> model)
        {
            return View(model);
        }
    }
}
