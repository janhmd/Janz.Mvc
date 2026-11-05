using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: mətn səhifələri (api/admin/pages). Ünvanlar sabit qalır, mətn və menyudakı yer dəyişir
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.Admin + "," + AppRoles.Editor)]
    public class PagesController : ApiControllerBase
    {
        private readonly SiteContent _siteContent;

        public PagesController(IHttpClientFactory httpClientFactory, IConfiguration configuration, SiteContent siteContent)
            : base(httpClientFactory, configuration)
        {
            _siteContent = siteContent;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (pages, status) = await GetAsync<List<PageVM>>("api/admin/pages");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (pages is null) ViewBag.Error = UnavailableMessage;

            return View(pages ?? new List<PageVM>());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (page, status) = await GetAsync<PageVM>($"api/admin/pages/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (page is null) return NotFound();

            return View(new PageEditVM
            {
                Id = page.Id,
                Slug = page.Slug,
                Title = page.Title,
                Body = page.Body,
                InTerms = page.InTerms,
                InFooter = page.InFooter,
                UpdatedAt = page.UpdatedAt
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PageEditVM model)
        {
            model.Id = id;
            if (!ModelState.IsValid) return View(model);

            var (success, errors, status) = await PutAsync($"api/admin/pages/{id}", new { title = model.Title, body = model.Body, inTerms = model.InTerms, inFooter = model.InFooter });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                foreach (var error in errors) ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            _siteContent.Reset();
            TempData["AdminSuccess"] = "The page was saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
