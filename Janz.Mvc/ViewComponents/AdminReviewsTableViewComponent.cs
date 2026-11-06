using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Reviews;

namespace Janz.Mvc.ViewComponents
{
    public class AdminReviewsTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminReviewListVM model)
        {
            return View(model);
        }
    }
}
