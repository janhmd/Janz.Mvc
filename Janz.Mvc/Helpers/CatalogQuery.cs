namespace Janz.Mvc.Helpers
{
    // Kataloq filtrləri linkdə saxlanılır (?acidity=low&sort=price-asc): forma seçilmiş vəziyyəti buradan bərpa edir
    public static class CatalogQuery
    {
        private static readonly Dictionary<string, string> Tabs = new(StringComparer.OrdinalIgnoreCase)
        {
            ["All"] = "",
            ["Recommended"] = "recommended",
            ["Popular"] = "popular",
            ["New"] = "new",
            ["Coffee of the week"] = "week",
            ["Running out"] = "running-out",
            ["Microlot"] = "microlot"
        };

        public static bool Has(HttpRequest request, string name, string value) =>
            request.Query[name].Any(m => string.Equals(m, value, StringComparison.OrdinalIgnoreCase));

        public static string TabValue(string title) => Tabs.TryGetValue(title, out var value) ? value : title.ToLowerInvariant();

        public static bool IsTab(HttpRequest request, string title) =>
            string.Equals(request.Query["tab"].FirstOrDefault() ?? string.Empty, TabValue(title), StringComparison.OrdinalIgnoreCase);

        // Teq adından slug (API-dəki SlugHelper kimi)
        public static string Slug(string name) =>
            System.Text.RegularExpressions.Regex.Replace(name.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');

        // Səhifədəki filtrlər API-yə olduğu kimi ötürülür, kateqoriya isə səhifənin özündəndir
        public static string ApiUrl(HttpRequest request, string? category)
        {
            var parts = request.Query
                .Where(m => !m.Key.Equals("category", StringComparison.OrdinalIgnoreCase))
                .SelectMany(m => m.Value.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => $"{Uri.EscapeDataString(m.Key)}={Uri.EscapeDataString(v!)}"))
                .ToList();
            if (!string.IsNullOrEmpty(category)) parts.Add("category=" + Uri.EscapeDataString(category));
            return "api/products" + (parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts));
        }

        public static int ActiveCount(HttpRequest request) =>
            request.Query.Where(m => m.Key is not ("tab" or "sort" or "q")).Sum(m => m.Value.Count(v => !string.IsNullOrWhiteSpace(v)));
    }
}
