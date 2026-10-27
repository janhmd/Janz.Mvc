namespace Janz.Mvc.ViewModels.Orders
{
    // Admin panel: sifariş, müştəri və keçə biləcəyi statuslar (api/admin/orders)
    public class AdminOrderVM
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryFullName { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public string DeliveryLine { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryPostalCode { get; set; } = string.Empty;
        public List<AdminOrderItemVM> Items { get; set; } = new List<AdminOrderItemVM>();

        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public List<string> NextStatuses { get; set; } = new List<string>();
    }
}
