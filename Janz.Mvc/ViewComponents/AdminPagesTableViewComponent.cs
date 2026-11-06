using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Pages;

namespace Janz.Mvc.ViewComponents
{
    public class AdminPagesTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<PageVM> model)
        {
            return View(model);
        }
    }
}
