using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Tiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: Home və kataloq səhifələrindəki şəkilli kartlar (api/admin/tiles)
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class TilesController : ApiControllerBase
    {
        private readonly SiteContent _siteContent;

        public TilesController(IHttpClientFactory httpClientFactory, IConfiguration configuration, SiteContent siteContent)
            : base(httpClientFactory, configuration)
        {
            _siteContent = siteContent;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (tiles, status) = await GetAsync<List<SiteTileVM>>("api/admin/tiles");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (tiles is null) ViewBag.Error = UnavailableMessage;

            return View(tiles ?? new List<SiteTileVM>());
        }

        [HttpGet]
        public IActionResult Create(string? placement)
        {
            var form = new SiteTileFormVM();
            if (placement is not null && SiteTileFormVM.Placements.Any(m => m.Key == placement)) form.Placement = placement;
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteTileFormVM model)
        {
            CheckImage(model.Image, true);
            if (!ModelState.IsValid) return View(model);

            using var form = BuildForm(model);
            var (created, errors, status) = await PostFormAsync<SiteTileVM>("api/admin/tiles", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                return View(model);
            }

            _siteContent.Reset();
            TempData["AdminSuccess"] = $"\"{created.Title}\" was added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (tile, status) = await GetAsync<SiteTileVM>($"api/admin/tiles/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (tile is null) return NotFound();

            return View(new SiteTileFormVM
            {
                Id = tile.Id,
                Placement = tile.Placement,
                Title = tile.Title,
                Link = tile.Link,
                SortOrder = tile.SortOrder,
                IsVisible = tile.IsVisible,
                CurrentImage = tile.ImagePath,
                HasVideo = !string.IsNullOrEmpty(tile.VideoPath)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteTileFormVM model)
        {
            model.Id = id;
            CheckImage(model.Image, false);
            if (!ModelState.IsValid) return await EditViewAsync(model);

            using var form = BuildForm(model);
            var (success, errors, status) = await PutFormAsync($"api/admin/tiles/{id}", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (!success)
            {
                AddErrors(errors);
                return await EditViewAsync(model);
            }

            _siteContent.Reset();
            TempData["AdminSuccess"] = $"\"{model.Title}\" was saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/tiles/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success)
            {
                _siteContent.Reset();
                TempData["AdminSuccess"] = "The tile was deleted.";
            }
            else
            {
                TempData["AdminError"] = string.Join(" ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // Xəta olub forma yenidən göstəriləndə indiki şəkil API-dən alınır
        private async Task<IActionResult> EditViewAsync(SiteTileFormVM model)
        {
            var (current, _) = await GetAsync<SiteTileVM>($"api/admin/tiles/{model.Id}");
            model.CurrentImage = current?.ImagePath ?? string.Empty;
            model.HasVideo = !string.IsNullOrEmpty(current?.VideoPath);
            return View(model);
        }

        // API də yoxlayır: böyük faylı API-yə göndərmədən burada saxlayırıq
        private void CheckImage(IFormFile? image, bool required)
        {
            if (image is null || image.Length == 0)
            {
                if (required) ModelState.AddModelError("Image", "Choose an image for the tile.");
                return;
            }

            if (image.Length > AdminRules.MaxImageBytes || !AdminRules.ImageTypes.Contains(image.ContentType))
            {
                ModelState.AddModelError("Image", "Only PNG, JPEG or WebP up to 2 MB.");
            }
        }

        private static MultipartFormDataContent BuildForm(SiteTileFormVM model)
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(model.Placement), "Placement" },
                { new StringContent(model.Title.Trim()), "Title" },
                { new StringContent(model.Link.Trim()), "Link" },
                { new StringContent(model.SortOrder.ToString()), "SortOrder" },
                { new StringContent(model.IsVisible ? "true" : "false"), "IsVisible" }
            };

            if (model.Image is not null && model.Image.Length > 0)
            {
                var content = new StreamContent(model.Image.OpenReadStream());
                content.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                form.Add(content, "Image", model.Image.FileName);
            }

            return form;
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
