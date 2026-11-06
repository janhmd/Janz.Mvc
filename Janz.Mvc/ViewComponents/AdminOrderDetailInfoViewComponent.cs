using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewComponents
{
    public class AdminOrderDetailInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminOrderVM model)
        {
            return View(model);
        }
    }
}
