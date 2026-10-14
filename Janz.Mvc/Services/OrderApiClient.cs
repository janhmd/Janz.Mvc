using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.Services
{
    public class OrderApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;

        public OrderApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("JanzApi");
        }

        public Task<OrderCallResult> CreateAsync(string token)
        {
            return SendAsync(HttpMethod.Post, "api/orders", token);
        }

        public Task<OrderCallResult> GetByIdAsync(string token, int id)
        {
            return SendAsync(HttpMethod.Get, "api/orders/" + id, token);
        }

        private async Task<OrderCallResult> SendAsync(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _http.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return OrderCallResult.Fail(new List<string> { "Please sign in again." }, unauthorized: true);
                }

                if (response.IsSuccessStatusCode)
                {
                    var order = await response.Content.ReadFromJsonAsync<OrderUIVM>(JsonOptions);
                    return OrderCallResult.Ok(order ?? new OrderUIVM());
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                return OrderCallResult.Fail(ReadErrors(json));
            }
            catch (HttpRequestException)
            {
                return OrderCallResult.Fail(new List<string> { "The API is not running." });
            }
            catch (JsonException)
            {
                return OrderCallResult.Fail(new List<string> { "The API did not return a valid response." });
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
