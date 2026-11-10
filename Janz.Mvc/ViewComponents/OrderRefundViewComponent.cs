using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Sifariş səhifəsində pulu geri qaytarma: sorğu formu və ya sorğunun vəziyyəti
    public class OrderRefundViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(OrderUIVM model)
        {
            return View(model);
        }
    }
}
