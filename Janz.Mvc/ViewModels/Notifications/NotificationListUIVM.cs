namespace Janz.Mvc.ViewModels.Notifications
{
    public class NotificationListUIVM
    {
        public int Unread { get; set; }
        public List<NotificationUIVM> Items { get; set; } = new List<NotificationUIVM>();
    }
}
