using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class OrderDetailReviewViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Orders.OrderUIVM model)
        {
            return View(model);
        }
    }
}
