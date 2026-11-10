namespace Janz.Mvc.ViewModels.Refunds
{
    // Admin panel: sorğu, müştəri və sifarişin cəmi (api/admin/refunds)
    public class AdminRefundVM : RefundUIVM
    {
        public static readonly string[] Statuses = { "Pending", "Approved", "Rejected" };

        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal OrderTotal { get; set; }
    }
}
