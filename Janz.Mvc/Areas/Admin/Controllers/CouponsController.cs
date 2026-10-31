using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Coupons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: kuponlar (api/admin/coupons). Yalnız Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class CouponsController : ApiControllerBase
    {
        public CouponsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            if (status is not null && !CouponListVM.Statuses.Contains(status)) status = null;

            var url = status is null ? "api/admin/coupons" : $"api/admin/coupons?status={Uri.EscapeDataString(status)}";
            var (coupons, apiStatus) = await GetAsync<List<CouponVM>>(url);
            if (apiStatus == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (coupons is null) ViewBag.Error = UnavailableMessage;

            return View(new CouponListVM { Coupons = coupons ?? new List<CouponVM>(), Status = status });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponCreateVM { UsageLimit = 100 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCreateVM model)
        {
            CheckForm(model.Type, model.Value, model.StartsAt, model.EndsAt);
            if (!ModelState.IsValid) return View(model);

            var body = ToBody(model.Code, model.Type, model.Value, model.MinOrder, model.UsageLimit, model.StartsAt, model.EndsAt, model.IsActive);
            var (created, errors, status) = await PostAsync<CouponVM>("api/admin/coupons", body);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                return View(model);
            }

            TempData["AdminSuccess"] = $"\"{created.Code}\" was added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (coupon, status) = await GetAsync<CouponVM>($"api/admin/coupons/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (coupon is null) return NotFound();

            return View(new CouponEditVM
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Type = coupon.Type,
                Value = coupon.Value,
                MinOrder = coupon.MinOrder,
                UsageLimit = coupon.UsageLimit,
                StartsAt = coupon.StartsAt,
                EndsAt = coupon.EndsAt,
                IsActive = coupon.IsActive,
                UsedCount = coupon.UsedCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponEditVM model)
        {
            model.Id = id;
            CheckForm(model.Type, model.Value, model.StartsAt, model.EndsAt);
            if (!ModelState.IsValid) return await EditViewAsync(model);

            var body = ToBody(model.Code, model.Type, model.Value, model.MinOrder, model.UsageLimit, model.StartsAt, model.EndsAt, model.IsActive);
            var (success, errors, status) = await PutAsync($"api/admin/coupons/{id}", body);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                AddErrors(errors);
                return await EditViewAsync(model);
            }

            TempData["AdminSuccess"] = "The changes were saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/coupons/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = "The coupon was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        // Xəta olub formu yenidən göstərəndə istifadə sayı da görünsün (formadan yox, API-dən alınır)
        private async Task<IActionResult> EditViewAsync(CouponEditVM model)
        {
            var (current, _) = await GetAsync<CouponVM>($"api/admin/coupons/{model.Id}");
            model.UsedCount = current?.UsedCount ?? 0;
            return View(model);
        }

        // API də yoxlayır: burada form səhvsiz getsin deyə
        private void CheckForm(string type, decimal value, DateTime? startsAt, DateTime? endsAt)
        {
            if (type != "Percent" && type != "Fixed")
            {
                ModelState.AddModelError("Type", "Choose the type: Percent or Fixed.");
            }

            if (type == "Percent" && value > 100)
            {
                ModelState.AddModelError("Value", "The percent can be at most 100.");
            }

            if (startsAt is not null && endsAt is not null && endsAt < startsAt)
            {
                ModelState.AddModelError("EndsAt", "The end date must be after the start date.");
            }
        }

        private static object ToBody(string? code, string type, decimal value, decimal minOrder, int? usageLimit, DateTime? startsAt, DateTime? endsAt, bool isActive)
        {
            return new
            {
                code = code?.Trim().ToUpperInvariant(),
                type,
                value,
                minOrder,
                usageLimit,
                startsAt,
                endsAt,
                isActive
            };
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
