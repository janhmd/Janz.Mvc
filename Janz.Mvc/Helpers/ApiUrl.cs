namespace Janz.Mvc.Helpers
{
    public static class ApiUrl
    {
        // Admin paneldən yüklənən şəkillər API-də saxlanılır (/uploads/...): onlara API-nin tam ünvanı lazımdır.
        // Saytın öz şəkilləri (/images/...) MVC-nin wwwroot-undadır və olduğu kimi qalır
        public static string ToAbsolute(string? baseUrl, string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return path;
            if (!path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return path;
            return (baseUrl ?? string.Empty).TrimEnd('/') + path;
        }
    }
}
