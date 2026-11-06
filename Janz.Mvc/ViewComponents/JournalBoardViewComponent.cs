using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class JournalBoardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Journal.JournalPageVM model)
        {
            return View(model);
        }
    }
}
