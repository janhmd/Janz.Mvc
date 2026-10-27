using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: kateqoriyalar (api/admin/categories). Yalnız Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class CategoriesController : ApiControllerBase
    {
        public CategoriesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (categories, status) = await GetAsync<List<CategoryVM>>("api/admin/categories");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (categories is null) ViewBag.Error = UnavailableMessage;

            return View((categories ?? new List<CategoryVM>()).OrderBy(m => m.Id).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormVM model)
        {
            model.Id = 0;
            if (!ModelState.IsValid) return View(model);

            var (created, errors, status) = await PostAsync<CategoryVM>("api/admin/categories", ToBody(model));
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                return View(model);
            }

            TempData["AdminSuccess"] = $"\"{created.Name}\" was added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (category, status) = await GetAsync<CategoryVM>($"api/admin/categories/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (category is null) return NotFound();

            return View(new CategoryFormVM { Id = category.Id, Name = category.Name, Slug = category.Slug });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormVM model)
        {
            model.Id = id;
            if (!ModelState.IsValid) return View(model);

            var (success, errors, status) = await PutAsync($"api/admin/categories/{id}", ToBody(model));
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                AddErrors(errors);
                return View(model);
            }

            TempData["AdminSuccess"] = "The changes were saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/categories/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // Məhsulu olan kateqoriya silinmir: API-nin mesajı olduğu kimi göstərilir
            if (success) TempData["AdminSuccess"] = "The category was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        // Slug boş qalsa API onu addan düzəldir
        private static object ToBody(CategoryFormVM model)
        {
            return new { name = model.Name?.Trim(), slug = string.IsNullOrWhiteSpace(model.Slug) ? null : model.Slug.Trim() };
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
