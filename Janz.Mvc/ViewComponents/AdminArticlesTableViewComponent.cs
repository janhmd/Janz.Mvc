using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class AdminArticlesTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Janz.Mvc.ViewModels.Articles.ArticleVM> model)
        {
            return View(model);
        }
    }
}
