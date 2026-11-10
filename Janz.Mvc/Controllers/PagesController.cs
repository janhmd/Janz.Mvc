using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Contact;
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
        public async Task<IActionResult> Contact(int? order)
        {
            // Google ev nömrəsini mətndən tapmırsa, appsettings-də "Contact:MapQuery" ilə dəqiq nöqtə (koordinat) yazılır
            ViewBag.MapQuery = Configuration["Contact:MapQuery"];
            // Sifariş səhifəsindən gələndə mövzu və mesajın başlanğıcı hazır olur
            ViewBag.Form = new ContactFormUIVM
            {
                Name = User.Identity?.IsAuthenticated == true ? User.Identity.Name ?? string.Empty : string.Empty,
                Topic = order is > 0 ? "Order" : string.Empty,
                Message = order is > 0 ? $"Order #{order}: " : string.Empty
            };
            return View(await _siteContent.SettingAsync());
        }

        // Əlaqə formu: mesaj API-yə yazılır (api/contact), cavabı admin e-poçtla göndərir
        [HttpPost("Contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormUIVM form)
        {
            if (ModelState.IsValid)
            {
                var (success, errors, _) = await PostAsync("api/contact", new { form.Name, form.Email, form.Topic, form.Message });
                if (success)
                {
                    TempData["Toast"] = User.Identity?.IsAuthenticated == true
                        ? "Thank you! We received your message. The answer will come by email and appear in your account under My messages."
                        : "Thank you! We received your message and will answer by email.";
                    return Redirect("/Contact");
                }

                foreach (var error in errors) ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.MapQuery = Configuration["Contact:MapQuery"];
            ViewBag.Form = form;
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
