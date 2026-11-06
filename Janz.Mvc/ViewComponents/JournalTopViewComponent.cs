using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class JournalTopViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Journal.JournalPageVM model)
        {
            return View(model);
        }
    }
}
