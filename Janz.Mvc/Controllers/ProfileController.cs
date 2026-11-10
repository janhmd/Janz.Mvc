using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Balance;
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
            var (balance, _) = await GetAsync<BalanceUIVM>("api/balance");
            var (saved, _) = await GetAsync<List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>>("api/articles/saved");
            var (notifications, _) = await GetAsync<Janz.Mvc.ViewModels.Notifications.NotificationListUIVM>("api/notifications");
            var userId = await CurrentUserIdAsync();
            var (community, _) = await GetAsync<Janz.Mvc.ViewModels.Community.CommunityProfileUIVM>($"api/community/profiles/{Uri.EscapeDataString(userId)}");
            var (blocked, _) = await GetAsync<List<Janz.Mvc.ViewModels.Community.CommunityPersonUIVM>>("api/community/blocked");
            var (products, _) = await GetAsync<List<ProductUIVM>>("api/products");
            var (messages, _) = await GetAsync<List<Janz.Mvc.ViewModels.Contact.MyMessageUIVM>>("api/contact/mine");

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
                Balance = balance ?? new BalanceUIVM(),
                SavedArticles = saved ?? new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>(),
                Profile = profile ?? new ProfileUIVM { FullName = User.Identity?.Name ?? string.Empty },
                Statistics = await StatisticsAsync(orders ?? new List<OrderUIVM>()),
                Notifications = notifications ?? new Janz.Mvc.ViewModels.Notifications.NotificationListUIVM(),
                Community = community ?? new Janz.Mvc.ViewModels.Community.CommunityProfileUIVM(),
                Blocked = blocked ?? new List<Janz.Mvc.ViewModels.Community.CommunityPersonUIVM>(),
                Messages = messages ?? new List<Janz.Mvc.ViewModels.Contact.MyMessageUIVM>(),
                Products = (products ?? new List<ProductUIVM>()).OrderBy(m => m.Name).ToList()
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
                                                       string? org, string? orgAddress, string? voen, string? tax)
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

            TempData["Toast"] = "Details saved";
            return Redirect(Url.Action(nameof(Index)) + "#details");
        }

        // "Change" next to the password: a link to choose a new password is emailed to the signed in user
        // Support cavabı kömək etməyibsə müştəri yenidən yazır; mesaj yenə support-un növbəsinə düşür
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageFollowUp(int id, string? text)
        {
            var (success, errors, status) = await PostAsync($"api/contact/{id}/follow-up", new { text });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "Your message was sent to support.";
            else TempData["MessageError"] = string.Join(" ", errors);

            return Redirect(Url.Action(nameof(Index)) + "#messages");
        }

        // Müştəri söhbəti bitirir: bundan sonra bu yazışmaya yeni yazı əlavə olunmur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MessageClose(int id)
        {
            var (success, errors, status) = await PostAsync($"api/contact/{id}/close", new { });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "The chat was ended.";
            else TempData["MessageError"] = string.Join(" ", errors);

            return Redirect(Url.Action(nameof(Index)) + "#messages");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordLink()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["AccountError"] = "We could not find your email. Please sign in again.";
                return Redirect(Url.Action(nameof(Index)) + "#details");
            }

            var (success, errors, status) = await PostAsync("api/account/forgot-password", new { email });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["PasswordLinkSent"] = email;
            else TempData["AccountError"] = string.Join(" ", errors);

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

        // "Top up balance" pəncərəsi: kartla (Stripe-a yönləndirilir) və ya sertifikat kodu ilə
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopUp(string? kind, decimal? amount, string? code)
        {
            if (kind == "certificate")
            {
                var (_, certificateErrors, certificateStatus) = await PostAsync<BalanceUIVM>("api/balance/certificate", new { code = code?.Trim() ?? string.Empty });
                if (certificateStatus == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
                if (certificateErrors.Length > 0) return LedgerError(certificateErrors);

                TempData["Toast"] = "The certificate is added to your balance";
                return Redirect(Url.Action(nameof(Index)) + "#ledger");
            }

            var (session, errors, status) = await PostAsync<PaymentSessionUIVM>("api/balance/top-up", new { amount = amount ?? 0 });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (session is null || string.IsNullOrEmpty(session.Url))
            {
                return LedgerError(errors.Length > 0 ? errors : new[] { "The payment could not be started." });
            }

            return Redirect(session.Url);
        }

        // Stripe ödənişdən sonra bura qaytarır, ödəniş API-də yoxlanılır
        [HttpGet]
        public async Task<IActionResult> TopUpPaid([FromQuery(Name = "session_id")] string? sessionId)
        {
            var (result, errors, status) = await PostAsync<TopUpResultUIVM>("api/balance/top-up/confirm", new { sessionId });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (result is null) return LedgerError(errors.Length > 0 ? errors : new[] { "The payment could not be checked." });

            TempData["Toast"] = result.IsPaid ? $"₼{result.Amount:0.00} is added to your balance" : "The payment is not finished yet.";
            return Redirect(Url.Action(nameof(Index)) + "#ledger");
        }

        private IActionResult LedgerError(string[] errors)
        {
            TempData["AccountError"] = string.Join(" ", errors);
            return Redirect(Url.Action(nameof(Index)) + "#ledger");
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
