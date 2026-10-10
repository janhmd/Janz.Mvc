using System.Net.Http.Json;
using System.Text.Json;
using Janz.Mvc.ViewModels.Categories;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.Services
{
    public class ProductApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;

        public ProductApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("JanzApi");
        }

        public Task<List<ProductUIVM>> GetAllAsync()
        {
            return GetListAsync<ProductUIVM>("api/products");
        }

        public Task<List<ProductUIVM>> GetByCategoryAsync(string slug)
        {
            return GetListAsync<ProductUIVM>("api/products/category/" + slug);
        }

        public Task<List<CategoryUIVM>> GetCategoriesAsync()
        {
            return GetListAsync<CategoryUIVM>("api/categories");
        }

        public async Task<ProductUIVM?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync("api/products/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ProductUIVM>(JsonOptions);
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            try
            {
                var items = await _http.GetFromJsonAsync<List<T>>(url, JsonOptions);
                return items ?? new List<T>();
            }
            catch (HttpRequestException)
            {
                return new List<T>();
            }
            catch (JsonException)
            {
                return new List<T>();
            }
        }
    }
}
