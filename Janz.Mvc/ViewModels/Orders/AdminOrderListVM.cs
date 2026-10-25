namespace Janz.Mvc.ViewModels.Orders
{
    public class AdminOrderListVM
    {
        public static readonly string[] Statuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };

        public List<AdminOrderUIVM> Orders { get; set; } = new List<AdminOrderUIVM>();
        public string? Status { get; set; }
        public string? Search { get; set; }
    }
}
