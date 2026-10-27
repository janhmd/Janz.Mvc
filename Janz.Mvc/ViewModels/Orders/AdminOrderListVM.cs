namespace Janz.Mvc.ViewModels.Orders
{
    public class AdminOrderListVM
    {
        public static readonly string[] Statuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };

        public List<AdminOrderVM> Orders { get; set; } = new List<AdminOrderVM>();
        public string? Status { get; set; }
        public string? Search { get; set; }
    }
}
