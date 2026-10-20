using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Header;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Saytın header-i: menyu, profil menyusu və səbətdəki məhsul sayı (hər səhifədə)
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HeaderViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
                        model.CartCount = cart?.Count ?? 0;
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

            return View(model);
        }
    }
}
