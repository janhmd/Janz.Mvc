using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewComponents
{
    public class AdminOrdersTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminOrderListVM model)
        {
            return View(model);
        }
    }
}
