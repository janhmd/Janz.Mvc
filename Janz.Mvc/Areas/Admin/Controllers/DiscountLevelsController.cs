using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.DiscountLevels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: endirim səviyyələri (api/admin/discount-levels). Yalnız Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class DiscountLevelsController : ApiControllerBase
    {
        public DiscountLevelsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (levels, status) = await GetAsync<List<DiscountLevelVM>>("api/admin/discount-levels");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (levels is null) ViewBag.Error = UnavailableMessage;

            return View(new DiscountLevelListVM { Levels = levels ?? new List<DiscountLevelVM>() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DiscountLevelListVM model)
        {
            var body = new
            {
                levels = model.Levels.Select(m => new
                {
                    id = m.Id,
                    name = m.Name?.Trim(),
                    minSpent = m.MinSpent,
                    percent = m.Percent,
                    perks = string.IsNullOrWhiteSpace(m.Perks) ? null : m.Perks.Trim()
                })
            };

            var (success, errors, status) = await PutAsync("api/admin/discount-levels", body);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (!success)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(model);
            }

            TempData["AdminSuccess"] = "The levels were saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
