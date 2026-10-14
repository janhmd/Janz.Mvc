using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Janz.Mvc.ViewModels.Cart;

namespace Janz.Mvc.Services
{
    public class CartApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;

        public CartApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("JanzApi");
        }

        public Task<CartCallResult> GetAsync(string token)
        {
            return SendAsync(HttpMethod.Get, "api/cart", token, null);
        }

        public Task<CartCallResult> AddAsync(string token, int productId, int quantity)
        {
            return SendAsync(HttpMethod.Post, "api/cart/items", token, new { productId, quantity });
        }

        public Task<CartCallResult> UpdateAsync(string token, int itemId, int quantity)
        {
            return SendAsync(HttpMethod.Put, "api/cart/items/" + itemId, token, new { quantity });
        }

        public Task<CartCallResult> RemoveAsync(string token, int itemId)
        {
            return SendAsync(HttpMethod.Delete, "api/cart/items/" + itemId, token, null);
        }

        private async Task<CartCallResult> SendAsync(HttpMethod method, string url, string token, object? body)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (body is not null)
            {
                request.Content = JsonContent.Create(body);
            }

            try
            {
                var response = await _http.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return CartCallResult.Fail(new List<string> { "Please sign in again." }, unauthorized: true);
                }

                if (response.IsSuccessStatusCode)
                {
                    var cart = await response.Content.ReadFromJsonAsync<CartUIVM>(JsonOptions);
                    return CartCallResult.Ok(cart ?? new CartUIVM());
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                return CartCallResult.Fail(ReadErrors(json));
            }
            catch (HttpRequestException)
            {
                return CartCallResult.Fail(new List<string> { "The API is not running." });
            }
            catch (JsonException)
            {
                return CartCallResult.Fail(new List<string> { "The API did not return a valid response." });
            }
        }

        private static List<string> ReadErrors(JsonElement json)
        {
            var errors = new List<string>();
            if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("errors", out var node))
            {
                if (node.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in node.EnumerateArray())
                    {
                        errors.Add(item.GetString() ?? string.Empty);
                    }
                }
                else if (node.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in node.EnumerateObject())
                    {
                        if (property.Value.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (var item in property.Value.EnumerateArray())
                        {
                            errors.Add(item.GetString() ?? string.Empty);
                        }
                    }
                }
            }

            errors.RemoveAll(string.IsNullOrWhiteSpace);
            if (errors.Count == 0)
            {
                errors.Add("Request failed.");
            }

            return errors;
        }
    }
}
