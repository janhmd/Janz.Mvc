using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Admin sifariş səhifəsində: müştərinin geri qaytarma sorğusu və qəbul/rədd formaları
    public class AdminOrderRefundViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminOrderVM model)
        {
            return View(model);
        }
    }
}
