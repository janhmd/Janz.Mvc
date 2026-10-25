using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Janz.Mvc.Controllers
{
    // Səbət. Məlumat API-dən gəlir: api/cart. Səhifənin özü giriş tələb etmir (boş səbət və tövsiyələr görünür),
    // dəyişikliklər isə giriş tələb edir: fetch() ilə gələn sorğuda cookie yönləndirmə əvəzinə 401 qaytarır
    public class CartController : ApiControllerBase
    {
        public CartController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CartPageVM();

            if (User.Identity?.IsAuthenticated == true)
            {
                var (cart, status) = await GetAsync<CartUIVM>("api/cart");
                if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
                model.Cart = cart ?? new CartUIVM();

                // Sifariş üçün çatdırılma ünvanları (alınmasa "ünvan əlavə et" linki görünür)
                var (addresses, _) = await GetAsync<List<AddressUIVM>>("api/addresses");
                model.Addresses = addresses ?? new List<AddressUIVM>();

                // Yeni əlavə olunan ünvan seçili gəlir, yoxsa birincisi
                var picked = TempData["SelectedAddressId"] as int?;
                model.SelectedAddressId = model.Addresses.Any(m => m.Id == picked)
                    ? picked
                    : model.Addresses.FirstOrDefault()?.Id;

                model.UserName = User.Identity?.Name ?? string.Empty;
                model.Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            }

            // Səbətdə olmayan 3 məhsul: "You might like"
            var inCart = model.Cart.Items.Select(m => m.ProductId).ToList();
            var (products, _) = await GetAsync<List<ProductUIVM>>("api/products");
            model.Suggestions = (products ?? new List<ProductUIVM>())
                .Where(m => !inCart.Contains(m.Id))
                .Take(3)
                .ToList();

            return View(model);
        }

        // Header-dəki səbət paneli: səbət dəyişəndə JS onu buradan təzədən götürür
        [HttpGet]
        public async Task<IActionResult> Panel()
        {
            return PartialView("_MiniCart", await LoadCartAsync());
        }

        // Kataloqdakı "səbətə at" düyməsi fetch() ilə çağırır: cavabda header üçün yeni say qayıdır
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity, int? variantId, string? grind)
        {
            var (cart, errors, status) = await PostAsync<CartUIVM>("api/cart/items", new { productId, quantity, variantId, grind });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (IsAjax())
            {
                return cart is not null
                    ? Ok(new { count = cart.Count })
                    : BadRequest(new { errors });
            }

            if (cart is null) TempData["CartError"] = string.Join(" ", errors);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            var (success, errors, status) = await PutAsync($"api/cart/items/{id}", new { quantity });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // Header panelindən gələndə yenilənmiş panel qaytarılır
            if (IsAjax())
            {
                return success ? PartialView("_MiniCart", await LoadCartAsync()) : BadRequest(new { errors });
            }

            if (!success) TempData["CartError"] = string.Join(" ", errors);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/cart/items/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (IsAjax())
            {
                return success ? PartialView("_MiniCart", await LoadCartAsync()) : BadRequest(new { errors });
            }

            if (!success) TempData["CartError"] = string.Join(" ", errors);
            return RedirectToAction(nameof(Index));
        }

        // Sifariş səhifəsindəki "Clear cart": səbətdəki bütün sətirlər silinir
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var cart = await LoadCartAsync();
            foreach (var item in cart.Items)
            {
                var (_, _, status) = await DeleteAsync($"api/cart/items/{item.Id}");
                if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Sifariş səhifəsində "New address": ünvan əlavə olunur və sifariş üçün seçilir
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(string? fullName, string? phone, string? line, string? city, string? postalCode)
        {
            var (address, errors, status) = await PostAsync<AddressUIVM>("api/addresses", new
            {
                fullName = fullName?.Trim() ?? string.Empty,
                phone = phone?.Trim() ?? string.Empty,
                line = line?.Trim() ?? string.Empty,
                city = city?.Trim() ?? string.Empty,
                postalCode = postalCode?.Trim() ?? string.Empty
            });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (address is null)
            {
                TempData["AddressError"] = string.Join(" ", errors);
            }
            else
            {
                TempData["SelectedAddressId"] = address.Id;
            }

            return RedirectToAction(nameof(Index));
        }

        // Giriş edilməyibsə və ya API cavab vermirsə boş səbət
        private async Task<CartUIVM> LoadCartAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return new CartUIVM();
            }

            var (cart, _) = await GetAsync<CartUIVM>("api/cart");
            return cart ?? new CartUIVM();
        }
    }
}
