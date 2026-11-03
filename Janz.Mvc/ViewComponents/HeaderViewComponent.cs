using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Header;
using Janz.Mvc.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Janz.Mvc.ViewComponents
{
    // Saytın header-i: menyu, profil menyusu, səbətdəki məhsul sayı və səbət paneli (hər səhifədə)
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public HeaderViewComponent(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        // Menyudakı "Reviews" yanındakı say: hər səhifədə API-yə getməmək üçün 5 dəqiqə yadda saxlanılır
        private async Task<int> ReviewCountAsync()
        {
            if (_cache.TryGetValue("review-count", out int count)) return count;
            try
            {
                var reviews = await _httpClientFactory.CreateClient("JanzApi").GetFromJsonAsync<ShopReviewsUIVM>("api/reviews?skip=0&take=1");
                count = reviews?.Total ?? 0;
                _cache.Set("review-count", count, TimeSpan.FromMinutes(5));
            }
            catch (HttpRequestException)
            {
            }
            catch (System.Text.Json.JsonException)
            {
            }
            return count;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new HeaderVM
            {
                SignedIn = User.Identity?.IsAuthenticated == true,
                UserName = User.Identity?.Name ?? string.Empty
            };

            if (model.SignedIn)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("JanzApi");
                    var response = await client.GetAsync("api/cart");
                    if (response.IsSuccessStatusCode)
                    {
                        var cart = await response.Content.ReadFromJsonAsync<CartUIVM>();
                        model.Cart = cart ?? new CartUIVM();
                        model.CartCount = model.Cart.Count;
                    }

                    var wish = await client.GetAsync("api/wishlist");
                    if (wish.IsSuccessStatusCode)
                    {
                        var ids = await wish.Content.ReadFromJsonAsync<List<int>>();
                        model.WishCount = ids?.Count ?? 0;
                    }

                    var notices = await client.GetAsync("api/notifications");
                    if (notices.IsSuccessStatusCode)
                    {
                        model.Notifications = await notices.Content.ReadFromJsonAsync<Janz.Mvc.ViewModels.Notifications.NotificationListUIVM>() ?? new Janz.Mvc.ViewModels.Notifications.NotificationListUIVM();
                    }
                }
                catch (HttpRequestException)
                {
                    // API işləmirsə say 0 göstərilir, səhifə yenə də açılır
                }
                catch (System.Text.Json.JsonException)
                {
                }
            }

            model.ReviewCount = await ReviewCountAsync();
            return View(model);
        }
    }
}
