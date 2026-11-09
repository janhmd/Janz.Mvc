namespace Janz.Mvc.ViewModels.Dashboard
{
    // api/admin/notifications cavabındakı bir sətir
    public class AdminNotificationVM
    {
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = "info";
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
