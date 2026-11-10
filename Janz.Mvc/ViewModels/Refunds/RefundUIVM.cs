namespace Janz.Mvc.ViewModels.Refunds
{
    // Pulun geri qaytarılması sorğusu (api/orders/{id} və api/admin/refunds)
    public class RefundUIVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;     // Pending | Approved | Rejected
        public string? StaffNote { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
