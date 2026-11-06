using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class AdminArticleCreateMainViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Articles.ArticleCreateVM model)
        {
            return View(model);
        }
    }
}
