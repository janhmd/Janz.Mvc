using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Rəylər səhifəsi (dizayn: Janz.Front/reviews.html).
    // Hələlik yalnız görünüşdür: rəylər və ulduz reytinqi API-də yazılanda (F7) bura qoşulacaq
    public class ReviewsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
