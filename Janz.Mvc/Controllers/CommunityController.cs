using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Community lenti (dizayn: Janz.Front/community.html).
    // Hələlik yalnız görünüşdür: postlar, şərhlər və blok nümunədir, məlumat backend-də yazılanda qoşulacaq
    public class CommunityController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
