using System.Net.Http.Json;
using System.Text.Json;

namespace Janz.Mvc.Services;

public class AccountApiClient
{
    private readonly HttpClient _http;

    public AccountApiClient(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("JanzApi");
    }

    public Task<AuthCallResult> RegisterAsync(string fullName, string email, string password, string repeatPassword)
    {
        return SendAsync("api/account/register", new
        {
            fullName,
            email,
            password,
            repeatPassword
        });
    }

    public Task<AuthCallResult> LoginAsync(string email, string password)
    {
        return SendAsync("api/account/login", new { email, password });
    }

    private async Task<AuthCallResult> SendAsync(string url, object body)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(url, body);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (response.IsSuccessStatusCode)
            {
                return AuthCallResult.Ok(
                    json.GetProperty("token").GetString() ?? string.Empty,
                    json.GetProperty("email").GetString() ?? string.Empty,
                    json.GetProperty("fullName").GetString() ?? string.Empty);
            }

            return AuthCallResult.Fail(ReadErrors(json));
        }
        catch (HttpRequestException)
        {
            return AuthCallResult.Fail("The API is not running.");
        }
        catch (JsonException)
        {
            return AuthCallResult.Fail("The API did not return a valid response.");
        }
    }

    private static List<string> ReadErrors(JsonElement json)
    {
        var errors = new List<string>();
        if (!json.TryGetProperty("errors", out var node))
        {
            if (json.TryGetProperty("title", out var title))
            {
                errors.Add(title.GetString() ?? "Request failed.");
            }
            return errors.Count == 0 ? ["Request failed."] : errors;
        }

        if (node.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in node.EnumerateArray())
            {
                var text = item.GetString();
                if (!string.IsNullOrWhiteSpace(text)) errors.Add(text);
            }
        }
        else if (node.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in node.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Array) continue;
                foreach (var item in property.Value.EnumerateArray())
                {
                    var text = item.GetString();
                    if (!string.IsNullOrWhiteSpace(text)) errors.Add(text);
                }
            }
        }

        return errors.Count == 0 ? ["Request failed."] : errors;
    }
}

public class AuthCallResult
{
    public bool Success { get; init; }
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Errors { get; init; } = [];

    public static AuthCallResult Ok(string token, string email, string fullName) => new()
    {
        Success = true,
        Token = token,
        Email = email,
        FullName = fullName
    };

    public static AuthCallResult Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = errors.ToList()
    };

    public static AuthCallResult Fail(List<string> errors) => new()
    {
        Success = false,
        Errors = errors
    };
}
