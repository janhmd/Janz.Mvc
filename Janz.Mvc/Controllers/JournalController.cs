using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Jurnal və məqalə səhifəsi (dizayn: Janz.Front/journal.html, article.html).
    // Hələlik yalnız görünüşdür: məqalələr nümunədir, backend-də yazılanda qoşulacaq
    public class JournalController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Article()
        {
            return View();
        }
    }
}
