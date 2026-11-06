using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    public class OrderDetailSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Janz.Mvc.ViewModels.Orders.OrderUIVM model)
        {
            return View(model);
        }
    }
}
