using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panelin ana səhifəsi: satışlar, sifarişlər, müştərilər və az qalan stok (api/admin/dashboard). Bütün işçilər görür
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Staff)]
    public class DashboardController : ApiControllerBase
    {
        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (dashboard, status) = await GetAsync<DashboardVM>("api/admin/dashboard");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // API cavab verməsə səhifə boş rəqəmlərlə açılır, yuxarıda xəbərdarlıq görünür
            if (dashboard is null)
            {
                ViewBag.Error = UnavailableMessage;
            }

            return View(dashboard ?? new DashboardVM());
        }
    }
}
