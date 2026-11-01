using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Products;
using Janz.Mvc.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Kataloq: hər kateqoriyanın öz səhifəsi var (dizayn fərqlidir), məhsullar API-dən gəlir: api/products/category/{slug}
    public class ProductsController : ApiControllerBase
    {
        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // Qəhvə
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Header-dəki axtarış (?q=) bütün kateqoriyalarda axtarır
            var query = Request.Query["q"].FirstOrDefault();
            ViewBag.Query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
            return View(await SearchAsync(ViewBag.Query is null ? "coffee" : null));
        }

        [HttpGet]
        public async Task<IActionResult> Tea()
        {
            return View(await SearchAsync("tea"));
        }

        [HttpGet]
        public async Task<IActionResult> Chocolate()
        {
            return View(await SearchAsync("chocolate"));
        }

        [HttpGet]
        public async Task<IActionResult> Sets()
        {
            return View(await SearchAsync("sets"));
        }

        // Məhsulun səhifəsi. id yoxdursa (köhnə link) ilk qəhvə göstərilir
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            ProductUIVM? product;
            if (id.HasValue)
            {
                (product, _) = await GetAsync<ProductUIVM>($"api/products/{id.Value}");
            }
            else
            {
                product = (await GetCategoryAsync("coffee")).FirstOrDefault();
            }

            if (product is null)
            {
                return NotFound();
            }

            // Eyni kateqoriyadan 3 başqa məhsul
            var related = (await GetCategoryAsync(product.CategorySlug))
                .Where(m => m.Id != product.Id)
                .Take(3)
                .ToList();

            var (reviews, _) = await GetAsync<ProductReviewsUIVM>($"api/products/{product.Id}/reviews");
            var (coupon, _) = User.Identity?.IsAuthenticated == true
                ? await GetAsync<Janz.Mvc.ViewModels.Cart.FirstOrderCouponUIVM>("api/coupons/first-order")
                : (null, null);
            var (wishlist, _) = User.Identity?.IsAuthenticated == true
                ? await GetAsync<List<int>>("api/wishlist")
                : (null, null);

            return View(new ProductPageVM
            {
                Coupon = coupon,
                InWishlist = wishlist?.Contains(product.Id) == true,
                Product = product,
                Reviews = reviews ?? new ProductReviewsUIVM(),
                Related = related
            });
        }

        private async Task<List<ProductUIVM>> SearchAsync(string? category)
        {
            var (products, _) = await GetAsync<List<ProductUIVM>>(CatalogQuery.ApiUrl(Request, category));
            return products ?? new List<ProductUIVM>();
        }

        private async Task<List<ProductUIVM>> GetCategoryAsync(string slug)
        {
            var (products, _) = await GetAsync<List<ProductUIVM>>($"api/products/category/{slug}");
            return products ?? new List<ProductUIVM>();
        }

        // Rəyi yazmaq və ya dəyişmək (API yoxlayır: məhsul çatdırılmış sifarişdə olmalıdır)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, int rating, string? text)
        {
            var (success, errors, status) = await PutAsync($"api/products/{id}/reviews", new { rating, text });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "Thank you! Your review was saved.";
            else TempData["ReviewError"] = string.Join(" ", errors);

            return Redirect($"/Products/Detail/{id}?tab=comments#comments");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/products/{id}/reviews");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "Your review was deleted.";
            else TempData["ReviewError"] = string.Join(" ", errors);

            return Redirect($"/Products/Detail/{id}?tab=comments#comments");
        }
    }
}
