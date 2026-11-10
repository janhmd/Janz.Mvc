using Janz.Mvc.ViewModels.Refunds;

namespace Janz.Mvc.ViewModels.Orders
{
    public class OrderUIVM
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string? CouponCode { get; set; }
        public string? LevelName { get; set; }
        public decimal Discount { get; set; }
        public decimal BalanceUsed { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryFullName { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public string DeliveryLine { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryPostalCode { get; set; } = string.Empty;
        // Pulun geri qaytarılması: ən son sorğu (yoxdursa null) və yeni sorğu göndərmək olarmı
        public RefundUIVM? Refund { get; set; }
        public bool CanRequestRefund { get; set; }
        public List<OrderItemUIVM> Items { get; set; } = new List<OrderItemUIVM>();
    }
}
