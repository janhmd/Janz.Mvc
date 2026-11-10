using Janz.Mvc.ViewModels.Common;
using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    public class HomeController : ApiControllerBase
    {
        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // Ana səhifə: qəhvə və setlər bölmələri (API işləmirsə bölmələr boş qalır)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (coffee, _) = await GetAsync<List<ProductUIVM>>("api/products/category/coffee");
            var (sets, _) = await GetAsync<List<ProductUIVM>>("api/products/category/sets");
            var (all, _) = await GetAsync<List<ProductUIVM>>("api/products");
            var products = all ?? new List<ProductUIVM>();
            var (articles, _) = await GetAsync<List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>>("api/articles?sort=new");
            var (reviews, _) = await GetAsync<Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM>("api/reviews?skip=0&take=2");
            var (coupon, _) = User.Identity?.IsAuthenticated == true
                ? await GetAsync<Janz.Mvc.ViewModels.Cart.FirstOrderCouponUIVM>("api/coupons/first-order")
                : (null, null);

            return View(new HomeIndexVM
            {
                Coupon = coupon,
                Reviews = reviews ?? new Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM(),
                Coffee = coffee ?? new List<ProductUIVM>(),
                Sets = sets ?? new List<ProductUIVM>(),
                Newest = products.OrderByDescending(m => m.CreatedAt).Take(4).ToList(),
                Popular = products.Where(m => m.SoldCount > 0).OrderByDescending(m => m.SoldCount).ThenByDescending(m => m.ReviewCount).Take(4).ToList(),
                LatestArticles = (articles ?? new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>()).OrderByDescending(m => m.PublishedAt).Take(3).ToList()
            });
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        // Xəta səhifəsi. Program.cs-dəki UseStatusCodePagesWithReExecute (404, 403 və s.) və UseExceptionHandler (500) bura yönləndirir
        public IActionResult Error(int? code)
        {
            var status = code ?? StatusCodes.Status500InternalServerError;
            Response.StatusCode = status;
            return View(new ErrorVM(status));
        }

        // Rolu uyğun olmayan istifadəçi (cookie AccessDeniedPath) buraya düşür
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View("Error", new ErrorVM(StatusCodes.Status403Forbidden));
        }
    }
}
