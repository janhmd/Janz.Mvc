using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: teqlər (api/admin/tags). Hamısı bir səhifədədir: yuxarıda yeni teq, cədvəldə adı dəyişmək və silmək
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class TagsController : ApiControllerBase
    {
        private const int MaxName = 100;

        public TagsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (model, result) = await LoadAsync();
            return result ?? View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? name)
        {
            var error = CheckName(name);
            if (error is null)
            {
                var (created, errors, status) = await PostAsync<TagUIVM>("api/admin/tags", new { name = name!.Trim() });
                if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

                if (created is not null)
                {
                    TempData["AdminSuccess"] = $"The tag \"{created.Name}\" was added.";
                    return RedirectToAction(nameof(Index));
                }

                error = string.Join(" ", errors);
            }

            // Səhv olanda yazılan ad itmir
            var (model, result) = await LoadAsync();
            if (result is not null) return result;
            model.NewName = name;
            ModelState.AddModelError(string.Empty, error);
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, string? name)
        {
            var error = CheckName(name);
            if (error is null)
            {
                // Slug göndərilmir: API onu yeni addan düzəldir
                var (success, errors, status) = await PutAsync($"api/admin/tags/{id}", new { name = name!.Trim() });
                if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
                if (status == HttpStatusCode.NotFound) return NotFound();

                if (success)
                {
                    TempData["AdminSuccess"] = "The tag was saved.";
                    return RedirectToAction(nameof(Index));
                }

                error = string.Join(" ", errors);
            }

            var (model, result) = await LoadAsync();
            if (result is not null) return result;
            model.EditedId = id;
            model.EditedName = name;
            ModelState.AddModelError(string.Empty, error);
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/tags/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = "The tag was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        private async Task<(TagListVM Model, IActionResult? Result)> LoadAsync()
        {
            var (tags, status) = await GetAsync<List<TagUIVM>>("api/admin/tags");
            if (status == HttpStatusCode.Unauthorized) return (new TagListVM(), await SessionExpiredAsync());
            if (tags is null) ViewBag.Error = UnavailableMessage;

            return (new TagListVM { Tags = (tags ?? new List<TagUIVM>()).OrderBy(m => m.Id).ToList() }, null);
        }

        private static string? CheckName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Enter the tag name.";
            if (name.Trim().Length > MaxName) return $"The tag name can be at most {MaxName} characters.";
            return null;
        }
    }
}
