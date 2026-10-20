using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Seçilmişlər (dizayn: Janz.Front/wishlist.html). Giriş tələb edir.
    // Hələlik yalnız görünüşdür: seçilmiş məhsulların API tərəfi yazılanda qoşulacaq
    [Authorize]
    public class WishlistController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
