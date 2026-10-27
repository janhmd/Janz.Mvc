using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Categories;
using Janz.Mvc.ViewModels.Products;
using Janz.Mvc.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: məhsullar (api/admin/products). Yalnız Admin və SuperAdmin
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class ProductsController : ApiControllerBase
    {
        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId, bool lowStock = false)
        {
            var (products, status) = await GetAsync<List<ProductVM>>("api/admin/products");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (products is null) ViewBag.Error = UnavailableMessage;

            var (categories, _) = await GetAsync<List<CategoryVM>>("api/admin/categories");

            var all = products ?? new List<ProductVM>();
            IEnumerable<ProductVM> list = all;

            // "#12" id ilə, qalanı ada görə axtarır
            if (!string.IsNullOrWhiteSpace(search))
            {
                var text = search.Trim();
                list = text.StartsWith('#') && int.TryParse(text[1..], out var id)
                    ? list.Where(m => m.Id == id)
                    : list.Where(m => m.Name.Contains(text, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId is not null)
            {
                list = list.Where(m => m.CategoryId == categoryId);
            }

            if (lowStock)
            {
                list = list.Where(m => m.Stock < AdminRules.LowStock).OrderBy(m => m.Stock);
            }
            else
            {
                list = list.OrderByDescending(m => m.Id);
            }

            return View(new ProductListVM
            {
                Products = list.ToList(),
                Categories = categories ?? new List<CategoryVM>(),
                Total = all.Count,
                Search = search,
                CategoryId = categoryId,
                LowStock = lowStock
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var (product, status) = await GetAsync<ProductVM>($"api/admin/products/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (product is null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductFormVM();
            await FillListsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM model)
        {
            model.Id = 0;
            PrepareVariants(model);
            if (model.Image is null || model.Image.Length == 0)
            {
                ModelState.AddModelError(nameof(model.Image), "Choose the main image.");
            }
            CheckFiles(model);

            if (!ModelState.IsValid)
            {
                await FillListsAsync(model);
                return View(model);
            }

            using var form = ToForm(model);
            var (created, errors, status) = await PostFormAsync<ProductVM>("api/admin/products", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (created is null)
            {
                AddErrors(errors);
                await FillListsAsync(model);
                return View(model);
            }

            TempData["AdminSuccess"] = $"\"{created.Name}\" was added.";
            return RedirectToAction(nameof(Detail), new { id = created.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (product, status) = await GetAsync<ProductVM>($"api/admin/products/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (product is null) return NotFound();

            var model = new ProductFormVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                TagIds = product.Tags.Select(m => m.Id).ToList(),
                OldPrice = product.OldPrice,
                Badge = product.Badge,
                IsVisible = product.IsVisible,
                Roast = product.Roast,
                Region = product.Region,
                Processing = product.Processing,
                Caffeine = product.Caffeine,
                Brewing = product.Brewing,
                Storage = product.Storage,
                BrewMethods = product.BrewMethods,
                Use = product.Use,
                Taste = product.Taste,
                Notes = string.Join(", ", product.Notes),
                Density = product.Density,
                Acidity = product.Acidity,
                TasteGroup = product.TasteGroup,
                Variants = product.Variants.Select(m => new ProductVariantFormVM
                {
                    Id = m.Id,
                    Format = m.Format,
                    Size = m.Size,
                    Price = m.Price,
                    OldPrice = m.OldPrice,
                    Stock = m.Stock,
                    KeepImages = m.Images.ToList(),
                    Images = m.Images
                }).ToList()
            };
            await FillListsAsync(model, product);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormVM model)
        {
            model.Id = id;
            PrepareVariants(model);
            CheckFiles(model);

            if (ModelState.IsValid)
            {
                using var form = ToForm(model);
                var (success, errors, status) = await PutFormAsync($"api/admin/products/{id}", form);
                if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
                if (status == HttpStatusCode.NotFound) return NotFound();

                if (success)
                {
                    TempData["AdminSuccess"] = "The changes were saved.";
                    return RedirectToAction(nameof(Detail), new { id });
                }

                AddErrors(errors);
            }

            // Səhv olanda köhnə şəkilləri yenidən göstərmək üçün məhsulu API-dən alırıq (yazılan sahələr qalır)
            var (product, _) = await GetAsync<ProductVM>($"api/admin/products/{id}");
            await FillListsAsync(model, product);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/admin/products/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // Sifarişdə olan məhsul silinmir: API-nin mesajı olduğu kimi göstərilir
            if (success) TempData["AdminSuccess"] = "The product was deleted.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Index));
        }

        // Kateqoriya və teq siyahıları, redaktədə isə hazırkı şəkillər
        private async Task FillListsAsync(ProductFormVM model, ProductVM? product = null)
        {
            var (categories, _) = await GetAsync<List<CategoryVM>>("api/admin/categories");
            var (tags, _) = await GetAsync<List<TagVM>>("api/admin/tags");
            model.Categories = categories ?? new List<CategoryVM>();
            model.Tags = tags ?? new List<TagVM>();

            if (product is not null)
            {
                model.ImagePath = product.ImagePath;
                model.Images = product.Images;

                // Səhv olanda variantın köhnə şəkilləri yenidən göstərilir (silinməsi seçilənlər də, "Undo" üçün)
                foreach (var variant in model.Variants.Where(m => m.Id is not null))
                {
                    variant.Images = product.Variants.FirstOrDefault(m => m.Id == variant.Id)?.Images ?? new List<string>();
                }
            }
        }

        // Variantlar varsa məhsulun öz qiyməti və stoku istifadə olunmur: ən ucuz variant və stokların cəmi göndərilir
        private void PrepareVariants(ProductFormVM model)
        {
            // Formatı və qiyməti boş qalan tam boş sətirlər nəzərə alınmır
            model.Variants = model.Variants
                .Where(m => !string.IsNullOrWhiteSpace(m.Format) || m.Price is not null || m.NewImages.Any(f => f.Length > 0) || m.Id is not null)
                .ToList();

            if (!model.HasVariants)
            {
                return;
            }

            ModelState.Remove(nameof(model.Price));
            ModelState.Remove(nameof(model.Stock));
            ModelState.Remove(nameof(model.OldPrice));

            if (model.Variants.Count > AdminRules.MaxVariants)
            {
                ModelState.AddModelError(string.Empty, $"A product can have at most {AdminRules.MaxVariants} variants.");
            }

            for (var i = 0; i < model.Variants.Count; i++)
            {
                var variant = model.Variants[i];
                var name = $"Variant {i + 1}";
                variant.NewImages = variant.NewImages.Where(m => m.Length > 0).ToList();

                if (string.IsNullOrWhiteSpace(variant.Format)) ModelState.AddModelError(string.Empty, $"{name}: choose the pack type.");
                if (variant.Price is not decimal price || price <= 0) ModelState.AddModelError(string.Empty, $"{name}: the price must be more than 0.");
                if (variant.OldPrice is decimal old && variant.Price is decimal p && old <= p) ModelState.AddModelError(string.Empty, $"{name}: the old price must be more than the price.");
                if (variant.Stock is not int stock || stock < 0) ModelState.AddModelError(string.Empty, $"{name}: the stock can't be empty or negative.");
                if (variant.KeepImages.Count + variant.NewImages.Count > AdminRules.MaxVariantImages)
                {
                    ModelState.AddModelError(string.Empty, $"{name}: at most {AdminRules.MaxVariantImages} images.");
                }
            }

            var duplicates = model.Variants
                .GroupBy(m => ((m.Format ?? "").Trim().ToLower(), (m.Size ?? "").Trim().ToLower()))
                .Any(g => g.Count() > 1);
            if (duplicates) ModelState.AddModelError(string.Empty, "Two variants have the same pack and size.");

            var priced = model.Variants.Where(m => m.Price is decimal p && p > 0).ToList();
            if (priced.Count > 0)
            {
                var cheapest = priced.OrderBy(m => m.Price).First();
                model.Price = cheapest.Price!.Value;
                model.OldPrice = cheapest.OldPrice;
            }
            model.Stock = model.Variants.Sum(m => Math.Max(m.Stock ?? 0, 0));
        }

        // Faylları API-yə göndərmədən yoxlayırıq: ölçü, tip və qalereyanın limiti
        private void CheckFiles(ProductFormVM model)
        {
            model.Gallery = model.Gallery.Where(m => m.Length > 0).ToList();

            var files = model.Gallery.ToList();
            if (model.Image is not null && model.Image.Length > 0) files.Add(model.Image);
            files.AddRange(model.Variants.SelectMany(m => m.NewImages));

            foreach (var file in files)
            {
                if (file.Length > AdminRules.MaxImageBytes)
                {
                    ModelState.AddModelError(string.Empty, $"\"{file.FileName}\" is larger than 2 MB.");
                }
                else if (!AdminRules.ImageTypes.Contains(file.ContentType))
                {
                    ModelState.AddModelError(string.Empty, $"\"{file.FileName}\" must be a PNG, JPG or WEBP image.");
                }
            }

            // Köhnə qiymət yalnız endirim varsa: indiki qiymətdən böyük olmalıdır (API də yoxlayır)
            if (model.OldPrice is decimal old && old <= model.Price)
            {
                ModelState.AddModelError(nameof(model.OldPrice), "The old price must be more than the price.");
            }

            if (!string.IsNullOrEmpty(model.Badge) && !ProductBadges.All.Contains(model.Badge))
            {
                ModelState.AddModelError(nameof(model.Badge), "Choose a badge from the list.");
            }

            if (model.Gallery.Count > AdminRules.MaxGallery)
            {
                ModelState.AddModelError(nameof(model.Gallery), $"The gallery can have at most {AdminRules.MaxGallery} images.");
            }
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        // API-nin gözlədiyi sahə adları: CreateProductDto / UpdateProductDto
        private static MultipartFormDataContent ToForm(ProductFormVM model)
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(model.Name ?? string.Empty), "Name" },
                { new StringContent(model.Description ?? string.Empty), "Description" },
                // API InvariantCulture ilə oxuyur: 29.90 vergül yox, nöqtə ilə getməlidir
                { new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price" },
                { new StringContent(model.Stock.ToString(CultureInfo.InvariantCulture)), "Stock" },
                { new StringContent(model.CategoryId.ToString(CultureInfo.InvariantCulture)), "CategoryId" }
            };

            // Boş sahələr göndərilmir: API onları boş (null) saxlayır
            AddText(form, "OldPrice", model.OldPrice?.ToString(CultureInfo.InvariantCulture));
            AddText(form, "Badge", model.Badge);
            AddText(form, "IsVisible", model.IsVisible ? "true" : "false");
            AddText(form, "Roast", model.Roast);
            AddText(form, "Region", model.Region);
            AddText(form, "Processing", model.Processing);
            AddText(form, "Caffeine", model.Caffeine);
            AddText(form, "Brewing", model.Brewing);
            AddText(form, "Storage", model.Storage);
            AddText(form, "Use", model.Use);
            AddText(form, "Taste", model.Taste);
            AddText(form, "Notes", model.Notes);
            AddText(form, "Density", model.Density?.ToString(CultureInfo.InvariantCulture));
            AddText(form, "Acidity", model.Acidity?.ToString(CultureInfo.InvariantCulture));
            AddText(form, "TasteGroup", model.TasteGroup);

            foreach (var method in model.BrewMethods.Where(m => ProductOptions.BrewMethods.Contains(m)).Distinct())
            {
                form.Add(new StringContent(method), "BrewMethods");
            }

            // Variantlar: Variants[0].Format ... API-dəki ProductVariantFormDto
            for (var i = 0; i < model.Variants.Count; i++)
            {
                var variant = model.Variants[i];
                var prefix = $"Variants[{i}].";
                AddText(form, prefix + "Id", variant.Id?.ToString(CultureInfo.InvariantCulture));
                AddText(form, prefix + "Format", variant.Format);
                AddText(form, prefix + "Size", variant.Size);
                AddText(form, prefix + "Price", variant.Price?.ToString(CultureInfo.InvariantCulture));
                AddText(form, prefix + "OldPrice", variant.OldPrice?.ToString(CultureInfo.InvariantCulture));
                AddText(form, prefix + "Stock", (variant.Stock ?? 0).ToString(CultureInfo.InvariantCulture));

                foreach (var path in variant.KeepImages.Distinct())
                {
                    form.Add(new StringContent(path), prefix + "KeepImages");
                }

                foreach (var file in variant.NewImages)
                {
                    AddFile(form, file, prefix + "NewImages");
                }
            }

            foreach (var tagId in model.TagIds.Distinct())
            {
                form.Add(new StringContent(tagId.ToString(CultureInfo.InvariantCulture)), "TagIds");
            }

            if (model.Image is not null && model.Image.Length > 0)
            {
                AddFile(form, model.Image, "Image");
            }

            foreach (var file in model.Gallery)
            {
                AddFile(form, file, "Gallery");
            }

            foreach (var path in model.RemoveImages.Distinct())
            {
                form.Add(new StringContent(path), "RemoveImages");
            }

            return form;
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
    }
}
