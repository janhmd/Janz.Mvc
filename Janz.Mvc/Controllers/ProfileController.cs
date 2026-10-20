using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Profile;
using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // İstifadəçinin kabineti (dizayn: Janz.Front/account.html): sifariş tarixçəsi, çatdırılma ünvanları.
    // Məlumat API-dən gəlir: api/orders, api/addresses. Community, Journal və Blacklist tabları hələlik yalnız görünüşdür
    [Authorize]
    public class ProfileController : ApiControllerBase
    {
        public ProfileController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? edit)
        {
            var (addresses, addressStatus) = await GetAsync<List<AddressUIVM>>("api/addresses");
            if (addressStatus == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            var (orders, orderStatus) = await GetAsync<List<OrderUIVM>>("api/orders");
            if (orderStatus == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // Biri alınmasa səhifə yenə də açılır, yuxarıda xəbərdarlıq görünür
            if (addresses is null || orders is null)
            {
                ViewBag.Error = UnavailableMessage;
            }

            return View(new ProfilePageVM
            {
                Addresses = addresses ?? new List<AddressUIVM>(),
                Orders = orders ?? new List<OrderUIVM>(),
                EditId = edit
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(string? fullName, string? phone, string? line, string? city, string? postalCode)
        {
            var (success, errors, status) = await PostAsync("api/addresses", AddressBody(fullName, phone, line, city, postalCode));
            return await AddressResultAsync(success, errors, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(int id, string? fullName, string? phone, string? line, string? city, string? postalCode)
        {
            var (success, errors, status) = await PutAsync($"api/addresses/{id}", AddressBody(fullName, phone, line, city, postalCode));
            return await AddressResultAsync(success, errors, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAddress(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/addresses/{id}");
            return await AddressResultAsync(success, errors, status);
        }

        private static object AddressBody(string? fullName, string? phone, string? line, string? city, string? postalCode)
        {
            return new
            {
                fullName = fullName?.Trim() ?? string.Empty,
                phone = phone?.Trim() ?? string.Empty,
                line = line?.Trim() ?? string.Empty,
                city = city?.Trim() ?? string.Empty,
                postalCode = postalCode?.Trim() ?? string.Empty
            };
        }

        // Ünvan dəyişikliyindən sonra kabinetə qayıdılır; xəta olarsa "My details" bölməsində göstərilir
        private async Task<IActionResult> AddressResultAsync(bool success, string[] errors, HttpStatusCode? status)
        {
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (!success)
            {
                TempData["AccountError"] = string.Join(" ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
