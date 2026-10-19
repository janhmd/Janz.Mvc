using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Janz.Mvc.ViewModels.Account;

namespace Janz.Mvc.Services
{
    public class AddressApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;

        public AddressApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("JanzApi");
        }

        public Task<AddressCallResult> GetAllAsync(string token)
        {
            return SendAsync(HttpMethod.Get, "api/addresses", token, null, list: true);
        }

        public Task<AddressCallResult> CreateAsync(string token, string fullName, string phone, string line, string city, string postalCode)
        {
            return SendAsync(HttpMethod.Post, "api/addresses", token, new { fullName, phone, line, city, postalCode }, list: false);
        }

        public Task<AddressCallResult> UpdateAsync(string token, int id, string fullName, string phone, string line, string city, string postalCode)
        {
            return SendAsync(HttpMethod.Put, "api/addresses/" + id, token, new { fullName, phone, line, city, postalCode }, list: false);
        }

        public Task<AddressCallResult> DeleteAsync(string token, int id)
        {
            return SendAsync(HttpMethod.Delete, "api/addresses/" + id, token, null, list: false);
        }

        private async Task<AddressCallResult> SendAsync(HttpMethod method, string url, string token, object? body, bool list)
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
                    return AddressCallResult.Fail(new List<string> { "Please sign in again." }, unauthorized: true);
                }

                if (response.IsSuccessStatusCode)
                {
                    if (list)
                    {
                        var addresses = await response.Content.ReadFromJsonAsync<List<AddressUIVM>>(JsonOptions);
                        return AddressCallResult.Ok(addresses ?? new List<AddressUIVM>());
                    }

                    if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                    {
                        return AddressCallResult.Ok();
                    }

                    var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
                    if (json.ValueKind == JsonValueKind.Object)
                    {
                        var address = JsonSerializer.Deserialize<AddressUIVM>(json.GetRawText(), JsonOptions);
                        return AddressCallResult.Ok(address ?? new AddressUIVM());
                    }

                    return AddressCallResult.Ok();
                }

                var errorJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                return AddressCallResult.Fail(ReadErrors(errorJson));
            }
            catch (HttpRequestException)
            {
                return AddressCallResult.Fail(new List<string> { "The API is not running." });
            }
            catch (JsonException)
            {
                return AddressCallResult.Fail(new List<string> { "The API did not return a valid response." });
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
