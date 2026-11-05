using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Journal;
using Janz.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Mətn səhifələri (Delivery and payment, Refunds, FAQ...): mətni admin paneldən yazılır (api/pages)
    public class PagesController : ApiControllerBase
    {
        public PagesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet("Pages/{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            var (page, status) = await GetAsync<SitePageUIVM>($"api/pages/{Uri.EscapeDataString(slug)}");
            if (page is null) return status == HttpStatusCode.NotFound ? NotFound() : RedirectToAction("Index", "Home");

            ViewBag.Blocks = ArticleBody.Parse(page.Body, new List<string>());
            return View(page);
        }
    }
}
