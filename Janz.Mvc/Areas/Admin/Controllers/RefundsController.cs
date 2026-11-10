using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Refunds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: pulun geri qaytarılması sorğuları (api/admin/refunds). Admin və Support qəbul edir
    // (məbləğ müştərinin balansına qayıdır) və ya rədd edir
    [Area("Admin")]
    [Authorize(Roles = AppRoles.OrderManagers)]
    public class RefundsController : ApiControllerBase
    {
        public RefundsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            if (status is not null && !AdminRefundVM.Statuses.Contains(status)) status = null;

            var (refunds, code) = await GetAsync<List<AdminRefundVM>>("api/admin/refunds?status=" + Uri.EscapeDataString(status ?? string.Empty));
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (refunds is null) ViewBag.Error = UnavailableMessage;

            ViewBag.Status = status;
            return View(refunds ?? new List<AdminRefundVM>());
        }

        // returnOrder doldurulubsa sifarişin səhifəsinə qayıdılır, yoxsa siyahıya
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, decimal? amount, int? returnOrder)
        {
            var (success, errors, code) = await PutAsync($"api/admin/refunds/{id}/approve", new { amount });
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (code == HttpStatusCode.NotFound) return NotFound();

            if (success) TempData["AdminSuccess"] = "The refund was accepted and the money was returned to the customer's balance.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return Back(returnOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? note, int? returnOrder)
        {
            var (success, errors, code) = await PutAsync($"api/admin/refunds/{id}/reject", new { note });
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (code == HttpStatusCode.NotFound) return NotFound();

            if (success) TempData["AdminSuccess"] = "The refund request was rejected. The customer sees your reason.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return Back(returnOrder);
        }

        private IActionResult Back(int? returnOrder)
        {
            return returnOrder is int orderId
                ? RedirectToAction("Detail", "Orders", new { id = orderId })
                : RedirectToAction(nameof(Index));
        }
    }
}
