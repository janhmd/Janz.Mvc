using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: sayt parametrləri, yəni əlaqə məlumatları (api/admin/settings). Yalnız SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class SettingsController : ApiControllerBase
    {
        private readonly SiteContent _siteContent;

        public SettingsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, SiteContent siteContent)
            : base(httpClientFactory, configuration)
        {
            _siteContent = siteContent;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (setting, status) = await GetAsync<SettingsEditVM>("api/admin/settings");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (setting is null) ViewBag.Error = UnavailableMessage;

            return View(setting ?? new SettingsEditVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SettingsEditVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, errors, status) = await PutAsync("api/admin/settings", model);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (!success)
            {
                foreach (var error in errors) ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            _siteContent.Reset();
            TempData["AdminSuccess"] = "The settings were saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
