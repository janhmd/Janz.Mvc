using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Coupons;
using Janz.Mvc.ViewModels.GiftCertificates;
using Janz.Mvc.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: hədiyyə sertifikatları (api/admin/gift-certificates). Yalnız Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class GiftCertificatesController : ApiControllerBase
    {
        public GiftCertificatesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            if (status is not null && !GiftCertificateListVM.Statuses.Contains(status)) status = null;

            var url = status is null ? "api/admin/gift-certificates" : $"api/admin/gift-certificates?status={Uri.EscapeDataString(status)}";
            var (certificates, apiStatus) = await GetAsync<List<GiftCertificateVM>>(url);
            if (apiStatus == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (certificates is null) ViewBag.Error = UnavailableMessage;

            return View(new GiftCertificateListVM { Certificates = certificates ?? new List<GiftCertificateVM>(), Status = status });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new GiftCertificateCreateVM { Amount = 50 };
            model.Audience.Customers = await CustomersAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiftCertificateCreateVM model)
        {
            if (!ModelState.IsValid) return await CreateViewAsync(model);

            var (created, errors, status) = await PostAsync<GiftCertificateVM>("api/admin/gift-certificates", ToBody(model.Code, model.Amount, model.ExpiresAt, model.IsActive));
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
            var (certificate, status) = await GetAsync<GiftCertificateVM>($"api/admin/gift-certificates/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (certificate is null) return NotFound();

            var customers = await CustomersAsync();
            return View(new GiftCertificateEditVM
            {
                Audience = new CouponAudienceVM { Customers = customers },
                Id = certificate.Id,
                Code = certificate.Code,
                Amount = certificate.Amount,
                ExpiresAt = certificate.ExpiresAt,
                IsActive = certificate.IsActive,
                UsedByEmail = certificate.UsedByEmail,
                UsedAt = certificate.UsedAt
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GiftCertificateEditVM model)
        {
            model.Id = id;
            if (!ModelState.IsValid) return await EditViewAsync(model);

            var (success, errors, status) = await PutAsync($"api/admin/gift-certificates/{id}", ToBody(model.Code, model.Amount, model.ExpiresAt, model.IsActive));
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
            var (success, errors, status) = await DeleteAsync($"api/admin/gift-certificates/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = "The certificate was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        // Xəta olub formu yenidən göstərəndə kimin istifadə etdiyi də görünsün (API-dən alınır)
        private async Task<IActionResult> EditViewAsync(GiftCertificateEditVM model)
        {
            var (current, _) = await GetAsync<GiftCertificateVM>($"api/admin/gift-certificates/{model.Id}");
            model.UsedByEmail = current?.UsedByEmail;
            model.UsedAt = current?.UsedAt;
            model.Audience.Customers = await CustomersAsync();
            return View(model);
        }

        private async Task<IActionResult> CreateViewAsync(GiftCertificateCreateVM model)
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

        // Sertifikat saxlanandan sonra seçilmişlərə bildiriş: nəticə admin mesajının sonuna əlavə olunur
        private async Task<string> NotifyAsync(int id, CouponAudienceVM audience)
        {
            if (audience.Mode != "all" && audience.Mode != "selected") return string.Empty;

            var (result, errors, _) = await PostAsync<CouponNotifyResultVM>($"api/admin/gift-certificates/{id}/notify", new { audience = audience.Mode, userIds = audience.UserIds });
            return result is null
                ? " But the code was not sent: " + string.Join(" ", errors)
                : $" The code was sent to {result.Sent} {(result.Sent == 1 ? "customer" : "customers")}.";
        }

        private static object ToBody(string? code, decimal amount, DateTime? expiresAt, bool isActive)
        {
            return new
            {
                code = code?.Trim().ToUpperInvariant(),
                amount,
                expiresAt,
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
