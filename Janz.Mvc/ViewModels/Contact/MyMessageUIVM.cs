namespace Janz.Mvc.ViewModels.Contact
{
    // api/contact/mine: müştərinin öz mesajı və aldığı cavab (hesabda "My messages")
    public class MyMessageUIVM
    {
        public int Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAnswered { get; set; }
        public string? Reply { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
