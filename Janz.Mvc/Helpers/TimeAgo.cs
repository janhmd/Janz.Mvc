namespace Janz.Mvc.Helpers
{
    // Məqalənin neçə gün əvvəl paylaşıldığı, qısa: Today, 1d ago, 10d ago, 3w ago, 2mo ago (chip-lər bir sətirdə qalsın)
    public static class TimeAgo
    {
        public static string Format(DateTime publishedAt)
        {
            var days = (DateTime.Now.Date - publishedAt.ToLocalTime().Date).Days;
            if (days <= 0) return "Today";
            if (days == 1) return "1d ago";
            if (days < 14) return $"{days}d ago";
            if (days < 60) return $"{days / 7}w ago";
            return $"{days / 30}mo ago";
        }
    }
}
