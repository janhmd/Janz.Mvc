using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Admin paneldən yüklənən şəkillər API-də saxlanılır (/uploads/...). Sayt https-dədir, API isə http:
    // brauzer https səhifədə http şəkli bloklayır, ona görə şəkil saytın öz ünvanından verilir və API-dən gətirilir
    public class UploadsController : Controller
    {
        private static readonly Dictionary<string, string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".png"] = "image/png",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".webp"] = "image/webp"
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public UploadsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/uploads/{**path}")]
        [ResponseCache(Duration = 604800)]
        public async Task<IActionResult> Image(string path)
        {
            // Yalnız şəkil faylları və yalnız uploads qovluğunun içi
            if (string.IsNullOrWhiteSpace(path) || path.Contains("..") || path.Contains('\\')) return NotFound();
            if (!ImageTypes.TryGetValue(Path.GetExtension(path), out var contentType)) return NotFound();

            try
            {
                var response = await _httpClientFactory.CreateClient("JanzApi").GetAsync("uploads/" + path, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode) return NotFound();
                return File(await response.Content.ReadAsByteArrayAsync(), contentType);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }
    }
}
