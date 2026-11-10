using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Mətn səhifələri (Delivery and payment, Refunds, FAQ...): mətni admin paneldən yazılır (api/pages)
    public class PagesController : ApiControllerBase
    {
        private readonly SiteContent _siteContent;

        public PagesController(IHttpClientFactory httpClientFactory, IConfiguration configuration, SiteContent siteContent)
            : base(httpClientFactory, configuration)
        {
            _siteContent = siteContent;
        }

        // Əlaqə səhifəsi: telefon, email, ünvan və xəritə admin paneldəki Settings-dən gəlir
        [HttpGet("Contact")]
        public async Task<IActionResult> Contact()
        {
            // Google ev nömrəsini mətndən tapmırsa, appsettings-də "Contact:MapQuery" ilə dəqiq nöqtə (koordinat) yazılır
            ViewBag.MapQuery = Configuration["Contact:MapQuery"];
            return View(await _siteContent.SettingAsync());
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
