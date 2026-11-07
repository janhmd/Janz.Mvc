namespace Janz.Mvc.Helpers
{
    // Məqalənin neçə gün əvvəl paylaşıldığı: bu gün, dünən, "N days ago", sonra həftə və ay
    public static class TimeAgo
    {
        public static string Format(DateTime publishedAt)
        {
            var days = (DateTime.Now.Date - publishedAt.ToLocalTime().Date).Days;
            if (days <= 0) return "Today";
            if (days == 1) return "Yesterday";
            if (days < 14) return $"{days} days ago";
            if (days < 60) return $"{days / 7} weeks ago";
            return $"{days / 30} months ago";
        }
    }
}
