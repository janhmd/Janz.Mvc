using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Products;
using Janz.Mvc.ViewModels.Profile;
using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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

            var (profile, _) = await GetAsync<ProfileUIVM>("api/profile");

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
                Profile = profile ?? new ProfileUIVM { FullName = User.Identity?.Name ?? string.Empty },
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

        // "Edit details" pəncərəsi: məlumatlar, istəyə görə şifrə dəyişmə
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDetails(string? fullName, string? phoneCode, string? phone, string? nickname,
                                                       string? org, string? orgAddress, string? voen, string? tax,
                                                       string? currentPassword, string? newPassword)
        {
            var number = phone?.Trim();
            var (success, errors, status) = await PutAsync("api/profile", new
            {
                fullName = fullName?.Trim() ?? string.Empty,
                phone = string.IsNullOrEmpty(number) ? null : $"{phoneCode} {number}",
                nickname = nickname?.Trim(),
                organizationName = org?.Trim(),
                organizationAddress = orgAddress?.Trim(),
                voen = voen?.Trim(),
                taxNumber = tax?.Trim()
            });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (!success) return DetailsError(errors);

            await RefreshNameAsync(fullName!.Trim());

            if (!string.IsNullOrEmpty(newPassword))
            {
                var (changed, passwordErrors, _) = await PutAsync("api/profile/password", new
                {
                    currentPassword = currentPassword ?? string.Empty,
                    newPassword
                });
                if (!changed) return DetailsError(passwordErrors);
            }

            TempData["Toast"] = "Details saved";
            return Redirect(Url.Action(nameof(Index)) + "#details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avatar(IFormFile? image)
        {
            if (image is null || image.Length == 0) return DetailsError(new[] { "Choose an image." });

            using var form = new MultipartFormDataContent();
            var content = new StreamContent(image.OpenReadStream());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
            form.Add(content, "Image", image.FileName);

            var (_, errors, status) = await PostFormAsync<ProfileUIVM>("api/profile/avatar", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (errors.Length > 0) return DetailsError(errors);

            return Redirect(Url.Action(nameof(Index)) + "#details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAvatar()
        {
            var (success, errors, status) = await DeleteAsync("api/profile/avatar");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (!success) return DetailsError(errors);

            return Redirect(Url.Action(nameof(Index)) + "#details");
        }

        private IActionResult DetailsError(string[] errors)
        {
            TempData["AccountError"] = string.Join(" ", errors);
            return Redirect(Url.Action(nameof(Index)) + "#details");
        }

        // Header-də ad cookie-dəki claim-dən oxunur: ad dəyişəndə cookie yeni adla yenidən yazılır (token eyni qalır)
        private async Task RefreshNameAsync(string fullName)
        {
            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!auth.Succeeded || auth.Principal is null || auth.Properties is null) return;

            var claims = auth.Principal.Claims.Where(m => m.Type != ClaimTypes.Name).ToList();
            claims.Add(new Claim(ClaimTypes.Name, fullName));
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, auth.Properties);
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
