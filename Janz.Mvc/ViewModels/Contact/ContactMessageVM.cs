namespace Janz.Mvc.ViewModels.Contact
{
    // api/admin/messages cavabındakı mesaj
    public class ContactMessageVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserAvatar { get; set; }
        public bool IsMember { get; set; }
        public bool IsAnswered { get; set; }
        public string? Reply { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public string? AnsweredByName { get; set; }
        public string? AnsweredByAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ContactEntryUIVM> Entries { get; set; } = new List<ContactEntryUIVM>();
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public bool CanFollowUp { get; set; }
    }
}
