using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Kataloq: hər kateqoriyanın öz səhifəsi var (dizayn fərqlidir), məhsullar API-dən gəlir: api/products/category/{slug}
    public class ProductsController : ApiControllerBase
    {
        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // Qəhvə
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await GetCategoryAsync("coffee"));
        }

        [HttpGet]
        public async Task<IActionResult> Tea()
        {
            return View(await GetCategoryAsync("tea"));
        }

        [HttpGet]
        public async Task<IActionResult> Chocolate()
        {
            return View(await GetCategoryAsync("chocolate"));
        }

        [HttpGet]
        public async Task<IActionResult> Sets()
        {
            return View(await GetCategoryAsync("sets"));
        }

        // Məhsulun səhifəsi. id yoxdursa (köhnə link) ilk qəhvə göstərilir
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            ProductUIVM? product;
            if (id.HasValue)
            {
                (product, _) = await GetAsync<ProductUIVM>($"api/products/{id.Value}");
            }
            else
            {
                product = (await GetCategoryAsync("coffee")).FirstOrDefault();
            }

            if (product is null)
            {
                return NotFound();
            }

            // Eyni kateqoriyadan 3 başqa məhsul
            var related = (await GetCategoryAsync(product.CategorySlug))
                .Where(m => m.Id != product.Id)
                .Take(3)
                .ToList();

            return View(new ProductPageVM
            {
                Product = product,
                Related = related
            });
        }

        private async Task<List<ProductUIVM>> GetCategoryAsync(string slug)
        {
            var (products, _) = await GetAsync<List<ProductUIVM>>($"api/products/category/{slug}");
            return products ?? new List<ProductUIVM>();
        }
    }
}
