using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panelin bildiriş mərkəzi (api/admin/notifications): gözləyən sifarişlər, şikayətlər, yeni rəylər, az qalan stok.
    // Hər işçi yalnız öz rolunun görə biləcəyi bildirişləri alır, filtrləmə API-dədir
    [Area("Admin")]
    [Authorize(Roles = AppRoles.OrderManagers)]
    public class NotificationsController : ApiControllerBase
    {
        private static readonly string[] Types = { "order", "report", "review", "stock" };

        public NotificationsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? type)
        {
            if (type is not null && !Types.Contains(type)) type = null;

            var (items, status) = await GetAsync<List<AdminNotificationVM>>("api/admin/notifications");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (items is null) ViewBag.Error = UnavailableMessage;

            ViewBag.Type = type;
            return View(items ?? new List<AdminNotificationVM>());
        }
    }
}
