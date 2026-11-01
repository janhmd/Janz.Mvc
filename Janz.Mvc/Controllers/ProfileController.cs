using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Products;
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
                EditId = edit,
                Statistics = await StatisticsAsync(orders ?? new List<OrderUIVM>())
            });
        }

        // Statistika ləğv olunmamış sifarişlərdən hesablanır, xərclənən məbləğ çatdırılmışlardandır (səviyyə də belə sayılır)
        private async Task<StatisticsUIVM> StatisticsAsync(List<OrderUIVM> orders)
        {
            var placed = orders.Where(m => m.Status != "Cancelled").ToList();
            var model = new StatisticsUIVM
            {
                OrderCount = placed.Count,
                ItemCount = placed.SelectMany(m => m.Items).Sum(m => m.Quantity),
                Spent = placed.Where(m => m.Status == "Delivered").Sum(m => m.Total),
                AverageOrder = placed.Count == 0 ? 0 : Math.Round(placed.Sum(m => m.Total) / placed.Count, 2),
                Saved = placed.Sum(m => m.Discount)
            };
            if (placed.Count == 0) return model;

            var (level, _) = await GetAsync<DiscountLevelUIVM>("api/discount-levels/me");
            model.Level = level;

            var topIds = placed.SelectMany(m => m.Items)
                .GroupBy(m => m.ProductId)
                .OrderByDescending(m => m.Sum(x => x.Quantity))
                .Select(m => m.Key)
                .Take(3)
                .ToList();
            var (products, _) = await GetAsync<List<ProductUIVM>>("api/products");
            var byId = (products ?? new List<ProductUIVM>()).ToDictionary(m => m.Id);
            model.TopProducts = topIds.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
            return model;
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
