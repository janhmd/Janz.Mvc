using Janz.Mvc.ViewModels.Orders;
using Janz.Mvc.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Reviews səhifəsi: mağaza və çatdırılma reytinqi, ulduzlar üzrə paylanma və rəylər (api/reviews)
    public class ReviewsController : ApiControllerBase
    {
        private const int PageSize = 12;

        public ReviewsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // ?shop=5&shop=4&delivery=5&q=fast: ulduza və mətnə görə süzgəc API-də işləyir
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int[]? shop = null, int[]? delivery = null, string? q = null)
        {
            page = Math.Max(1, page);
            var shopStars = (shop ?? Array.Empty<int>()).Where(m => m is >= 1 and <= 5).Distinct().OrderByDescending(m => m).ToList();
            var deliveryStars = (delivery ?? Array.Empty<int>()).Where(m => m is >= 1 and <= 5).Distinct().OrderByDescending(m => m).ToList();
            var search = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

            var url = $"api/reviews?skip=0&take={Math.Min(page * PageSize, 50)}";
            if (shopStars.Count > 0) url += "&shop=" + string.Join(",", shopStars);
            if (deliveryStars.Count > 0) url += "&delivery=" + string.Join(",", deliveryStars);
            if (search is not null) url += "&search=" + Uri.EscapeDataString(search);

            // Əvvəlki səhifələr də göstərilir: "Show more" siyahını uzadır
            var (reviews, _) = await GetAsync<ShopReviewsUIVM>(url);
            var model = reviews ?? new ShopReviewsUIVM();
            model.Page = page;
            model.ShopStars = shopStars;
            model.DeliveryStars = deliveryStars;
            model.Search = search;
            model.HasMore = model.Items.Count < model.Matching && page * PageSize < 50;

            // "Write a review": girişli müştərinin ən son çatdırılmış sifarişi (rəy sifariş səhifəsində yazılır)
            if (User.Identity?.IsAuthenticated == true)
            {
                var (orders, _) = await GetAsync<List<OrderUIVM>>("api/orders");
                ViewBag.ReviewOrderId = orders?.Where(m => m.Status == "Delivered").OrderByDescending(m => m.CreatedAt).Select(m => (int?)m.Id).FirstOrDefault();
            }

            return View(model);
        }
    }
}
