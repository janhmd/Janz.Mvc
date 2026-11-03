using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Seçilmişlər (dizayn: Janz.Front/wishlist.html). API Id-ləri qaytarır, məhsullar kataloqdan götürülür
    [Authorize]
    public class WishlistController : ApiControllerBase
    {
        public WishlistController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (ids, status) = await GetAsync<List<int>>("api/wishlist");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            var model = new WishlistPageVM();
            if (ids is not null && ids.Count > 0)
            {
                // Gizlədilmiş və ya silinmiş məhsul kataloqda yoxdur, ona görə siyahıya düşmür
                var (products, _) = await GetAsync<List<ProductUIVM>>("api/products");
                var byId = (products ?? new List<ProductUIVM>()).ToDictionary(m => m.Id);
                model.Products = ids.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
            }

            var (notes, _) = await GetAsync<List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>>("api/articles/saved");
            model.Notes = notes ?? new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>();

            return View(model);
        }

        // Ürək düyməsi (fetch): seçir və ya çıxarır, seçilmişlərin sayını qaytarır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int productId, bool add)
        {
            var (success, errors, status) = add
                ? await PostAsync($"api/wishlist/{productId}", new { })
                : await DeleteAsync($"api/wishlist/{productId}");
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            var (ids, _) = await GetAsync<List<int>>("api/wishlist");
            return Ok(new { count = ids?.Count ?? 0 });
        }
    }
}
