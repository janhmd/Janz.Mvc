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

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            page = Math.Max(1, page);
            // Əvvəlki səhifələr də göstərilir: "Show more" siyahını uzadır
            var (reviews, _) = await GetAsync<ShopReviewsUIVM>($"api/reviews?skip=0&take={Math.Min(page * PageSize, 50)}");
            var model = reviews ?? new ShopReviewsUIVM();
            model.Page = page;
            model.HasMore = model.Items.Count < model.Total && page * PageSize < 50;
            return View(model);
        }
    }
}
