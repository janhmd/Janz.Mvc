using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Müqayisə səhifəsi (dizayn: Janz.Front/compare.html). Məhsullar API-dən gəlir: api/products.
    // Hansı məhsulların seçildiyi brauzerdə saxlanılır (janz-compare-ids): giriş tələb etmir
    public class CompareController : ApiControllerBase
    {
        public CompareController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (products, _) = await GetAsync<List<ProductUIVM>>("api/products");
            var list = products ?? new List<ProductUIVM>();

            // Şəkillər səhifədə JS ilə çəkilir: yüklənmiş şəkillərə API-nin tam ünvanı əvvəlcədən yazılır
            var apiBase = Configuration["ApiSettings:BaseUrl"];
            foreach (var product in list)
            {
                product.ImagePath = ApiUrl.ToAbsolute(apiBase, product.ImagePath);
            }

            if (products is null)
            {
                ViewBag.Error = UnavailableMessage;
            }

            return View(list);
        }
    }
}
