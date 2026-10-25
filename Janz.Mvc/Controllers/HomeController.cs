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
            var (reviews, _) = await GetAsync<Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM>("api/reviews?skip=0&take=2");

            return View(new HomeIndexVM
            {
                Reviews = reviews ?? new Janz.Mvc.ViewModels.Reviews.ShopReviewsUIVM(),
                Coffee = coffee ?? new List<ProductUIVM>(),
                Sets = sets ?? new List<ProductUIVM>()
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
