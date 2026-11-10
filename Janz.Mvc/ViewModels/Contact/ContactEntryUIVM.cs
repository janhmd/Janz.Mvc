namespace Janz.Mvc.ViewModels.Contact
{
    // Yazışmanın davamı: cavabdan sonrakı yazılar (support və müştəri növbə ilə)
    public class ContactEntryUIVM
    {
        public bool IsStaff { get; set; }
        public string? StaffName { get; set; }
        public string? StaffAvatar { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
