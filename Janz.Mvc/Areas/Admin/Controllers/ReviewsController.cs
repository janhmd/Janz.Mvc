using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: məhsul və mağaza rəyləri (api/admin/reviews). Gizlədilən rəy saytda görünmür
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class ReviewsController : ApiControllerBase
    {
        private static readonly string[] Types = { "product", "shop" };

        public ReviewsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? type, bool? hidden, string? search)
        {
            if (type is not null && !Types.Contains(type)) type = null;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            var url = "api/admin/reviews?type=" + Uri.EscapeDataString(type ?? string.Empty)
                + "&search=" + Uri.EscapeDataString(search ?? string.Empty)
                + (hidden is null ? string.Empty : "&hidden=" + (hidden.Value ? "true" : "false"));
            var (reviews, status) = await GetAsync<List<AdminReviewVM>>(url);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (reviews is null) ViewBag.Error = UnavailableMessage;

            return View(new AdminReviewListVM { Reviews = reviews ?? new List<AdminReviewVM>(), Type = type, Hidden = hidden, Search = search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Hide(string type, int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/reviews/{type}/{id}/hide", new { }), "The review was hidden from the site.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Show(string type, int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/reviews/{type}/{id}/show", new { }), "The review is visible on the site again.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Delete(string type, int id, string? returnUrl) => SendAsync(() => DeleteAsync($"api/admin/reviews/{type}/{id}"), "The review was deleted.", returnUrl);

        // Filtr itməsin deyə əvvəlki siyahıya qayıdır
        private async Task<IActionResult> SendAsync(Func<Task<(bool Success, string[] Errors, HttpStatusCode? Status)>> call, string message, string? returnUrl)
        {
            var (success, errors, status) = await call();
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = message;
            else TempData["AdminError"] = status == HttpStatusCode.NotFound ? "The review was not found." : string.Join(" ", errors);

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction(nameof(Index));
        }
    }
}
