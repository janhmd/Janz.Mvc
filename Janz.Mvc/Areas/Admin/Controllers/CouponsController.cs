using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Coupons;
using Janz.Mvc.ViewModels.Users;
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
        public async Task<IActionResult> Create()
        {
            var model = new CouponCreateVM { UsageLimit = 100 };
            model.Audience.Customers = await CustomersAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCreateVM model)
        {
            CheckForm(model.Type, model.Value, model.StartsAt, model.EndsAt);
            if (!ModelState.IsValid) return await CreateViewAsync(model);

            var body = ToBody(model.Code, model.Type, model.Value, model.MinOrder, model.UsageLimit, model.StartsAt, model.EndsAt, model.IsActive, model.IsFirstOrder);
            var (created, errors, status) = await PostAsync<CouponVM>("api/admin/coupons", body);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                return await CreateViewAsync(model);
            }

            TempData["AdminSuccess"] = $"\"{created.Code}\" was added." + await NotifyAsync(created.Id, model.Audience);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (coupon, status) = await GetAsync<CouponVM>($"api/admin/coupons/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (coupon is null) return NotFound();

            var customers = await CustomersAsync();
            return View(new CouponEditVM
            {
                Audience = new CouponAudienceVM { Customers = customers },
                Id = coupon.Id,
                Code = coupon.Code,
                Type = coupon.Type,
                Value = coupon.Value,
                MinOrder = coupon.MinOrder,
                UsageLimit = coupon.UsageLimit,
                StartsAt = coupon.StartsAt,
                EndsAt = coupon.EndsAt,
                IsActive = coupon.IsActive,
                IsFirstOrder = coupon.IsFirstOrder,
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

            var body = ToBody(model.Code, model.Type, model.Value, model.MinOrder, model.UsageLimit, model.StartsAt, model.EndsAt, model.IsActive, model.IsFirstOrder);
            var (success, errors, status) = await PutAsync($"api/admin/coupons/{id}", body);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                AddErrors(errors);
                return await EditViewAsync(model);
            }

            TempData["AdminSuccess"] = "The changes were saved." + await NotifyAsync(id, model.Audience);
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
            model.Audience.Customers = await CustomersAsync();
            return View(model);
        }

        private async Task<IActionResult> CreateViewAsync(CouponCreateVM model)
        {
            model.Audience.Customers = await CustomersAsync();
            return View(model);
        }

        // Bloklanmamış müştərilər (seçim siyahısı üçün)
        private async Task<List<UserAdminVM>> CustomersAsync()
        {
            var (users, _) = await GetAsync<List<UserAdminVM>>("api/admin/users?role=User&blocked=false");
            return (users ?? new List<UserAdminVM>()).OrderBy(m => m.FullName).ToList();
        }

        // Kupon saxlanandan sonra seçilmişlərə bildiriş: nəticə admin mesajının sonuna əlavə olunur
        private async Task<string> NotifyAsync(int id, CouponAudienceVM audience)
        {
            if (audience.Mode != "all" && audience.Mode != "selected") return string.Empty;

            var (result, errors, _) = await PostAsync<CouponNotifyResultVM>($"api/admin/coupons/{id}/notify", new { audience = audience.Mode, userIds = audience.UserIds });
            return result is null
                ? " But the code was not sent: " + string.Join(" ", errors)
                : $" The code was sent to {result.Sent} {(result.Sent == 1 ? "customer" : "customers")}.";
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

        private static object ToBody(string? code, string type, decimal value, decimal minOrder, int? usageLimit, DateTime? startsAt, DateTime? endsAt, bool isActive, bool isFirstOrder)
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
                isActive,
                isFirstOrder
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
