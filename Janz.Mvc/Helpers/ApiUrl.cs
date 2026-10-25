namespace Janz.Mvc.Helpers
{
    public static class ApiUrl
    {
        // Admin paneldən yüklənən şəkillər API-dədir (/uploads/...), amma sayt onları öz ünvanından verir (UploadsController):
        // https səhifədə http API-dən şəkil bloklanır. Saytın öz şəkilləri (/images/...) və tam ünvanlar olduğu kimi qalır
        public static string ToAbsolute(string? baseUrl, string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            return path;
        }
    }
}
