using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: sifarişlər (api/admin/orders). Support da görür və statusu dəyişir
    [Area("Admin")]
    [Authorize(Roles = AppRoles.OrderManagers)]
    public class OrdersController : ApiControllerBase
    {
        public OrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status, string? search)
        {
            if (status is not null && !AdminOrderListVM.Statuses.Contains(status)) status = null;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            var url = "api/admin/orders?status=" + Uri.EscapeDataString(status ?? string.Empty)
                + "&search=" + Uri.EscapeDataString(search ?? string.Empty);
            var (orders, code) = await GetAsync<List<AdminOrderUIVM>>(url);
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (orders is null) ViewBag.Error = UnavailableMessage;

            return View(new AdminOrderListVM
            {
                Orders = (orders ?? new List<AdminOrderUIVM>()).OrderByDescending(m => m.CreatedAt).ToList(),
                Status = status,
                Search = search
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var (order, code) = await GetAsync<AdminOrderUIVM>($"api/admin/orders/{id}");
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (order is null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Status(int id, string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                TempData["AdminError"] = "Choose the new status.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var (success, errors, code) = await PutAsync($"api/admin/orders/{id}/status", new { status });
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (code == HttpStatusCode.NotFound) return NotFound();

            if (success) TempData["AdminSuccess"] = $"Order #{id} is now {status}.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
