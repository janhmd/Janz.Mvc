using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: jurnal məqalələri (api/admin/articles). Redaktor, Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.Admin + "," + AppRoles.Editor)]
    public class ArticlesController : ApiControllerBase
    {
        public ArticlesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (articles, status) = await GetAsync<List<ArticleVM>>("api/admin/articles");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (articles is null) ViewBag.Error = UnavailableMessage;

            return View(articles ?? new List<ArticleVM>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ArticleCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCreateVM model)
        {
            CheckFiles(model.Cover, model.Images, true);
            if (!ModelState.IsValid) return View(model);

            using var form = new MultipartFormDataContent();
            AddFields(form, model.Title, model.Slug, model.Topic, model.Lead, model.Body, model.Tags, model.AuthorName, model.AuthorRole, model.IsLearning, model.Series, model.IsPublished);
            AddFiles(form, model.Cover, model.Images);

            var (created, errors, status) = await PostFormAsync<ArticleVM>("api/admin/articles", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                return View(model);
            }

            TempData["AdminSuccess"] = $"\"{created.Title}\" was added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (article, status) = await GetAsync<ArticleVM>($"api/admin/articles/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (article is null) return NotFound();

            return View(new ArticleEditVM
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Topic = article.Topic,
                Lead = article.Lead,
                Body = article.Body,
                Tags = article.Tags,
                AuthorName = article.AuthorName,
                AuthorRole = article.AuthorRole,
                IsLearning = article.IsLearning,
                Series = article.Series,
                IsPublished = article.IsPublished,
                CoverPath = article.CoverPath,
                CurrentImages = article.Images
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleEditVM model)
        {
            model.Id = id;
            CheckFiles(model.Cover, model.Images, false);
            if (!ModelState.IsValid) return await EditViewAsync(model);

            using var form = new MultipartFormDataContent();
            AddFields(form, model.Title, model.Slug, model.Topic, model.Lead, model.Body, model.Tags, model.AuthorName, model.AuthorRole, model.IsLearning, model.Series, model.IsPublished);
            AddFiles(form, model.Cover, model.Images);
            foreach (var path in model.RemoveImages.Distinct())
            {
                form.Add(new StringContent(path), "RemoveImages");
            }

            var (success, errors, status) = await PutFormAsync($"api/admin/articles/{id}", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                AddErrors(errors);
                return await EditViewAsync(model);
            }

            TempData["AdminSuccess"] = "The changes were saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/articles/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = "The article was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        // Xəta olub forma yenidən göstəriləndə indiki şəkillər API-dən alınır
        private async Task<IActionResult> EditViewAsync(ArticleEditVM model)
        {
            var (current, _) = await GetAsync<ArticleVM>($"api/admin/articles/{model.Id}");
            model.CoverPath = current?.CoverPath ?? string.Empty;
            model.CurrentImages = current?.Images ?? new List<string>();
            return View(model);
        }

        // API də yoxlayır: böyük faylı API-yə göndərmədən burada saxlayırıq
        private void CheckFiles(IFormFile? cover, List<IFormFile> images, bool coverRequired)
        {
            if (coverRequired && (cover is null || cover.Length == 0))
            {
                ModelState.AddModelError("Cover", "Choose a cover image.");
            }

            foreach (var file in images.Append(cover).Where(m => m is not null && m.Length > 0))
            {
                if (file!.Length > AdminRules.MaxImageBytes || !AdminRules.ImageTypes.Contains(file.ContentType))
                {
                    ModelState.AddModelError(string.Empty, $"\"{file.FileName}\": only PNG, JPEG or WebP up to 2 MB.");
                }
            }
        }

        private static void AddFields(MultipartFormDataContent form, string? title, string? slug, string? topic, string? lead, string? body,
                                      List<string> tags, string? authorName, string? authorRole, bool isLearning, string? series, bool isPublished)
        {
            form.Add(new StringContent(title?.Trim() ?? string.Empty), "Title");
            AddText(form, "Slug", slug);
            form.Add(new StringContent(topic ?? string.Empty), "Topic");
            form.Add(new StringContent(lead?.Trim() ?? string.Empty), "Lead");
            form.Add(new StringContent(body ?? string.Empty), "Body");
            foreach (var tag in tags.Distinct())
            {
                form.Add(new StringContent(tag), "Tags");
            }
            AddText(form, "AuthorName", authorName);
            AddText(form, "AuthorRole", authorRole);
            form.Add(new StringContent(isLearning ? "true" : "false"), "IsLearning");
            AddText(form, "Series", series);
            form.Add(new StringContent(isPublished ? "true" : "false"), "IsPublished");
        }

        private static void AddFiles(MultipartFormDataContent form, IFormFile? cover, List<IFormFile> images)
        {
            if (cover is not null && cover.Length > 0)
            {
                AddFile(form, cover, "Cover");
            }

            foreach (var file in images.Where(m => m.Length > 0))
            {
                AddFile(form, file, "Images");
            }
        }

        private static void AddText(MultipartFormDataContent form, string name, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                form.Add(new StringContent(value.Trim()), name);
            }
        }

        private static void AddFile(MultipartFormDataContent form, IFormFile file, string name)
        {
            var content = new StreamContent(file.OpenReadStream());
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(content, name, file.FileName);
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
