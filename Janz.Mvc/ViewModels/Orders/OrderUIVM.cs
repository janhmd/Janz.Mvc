namespace Janz.Mvc.ViewModels.Orders
{
    public class OrderUIVM
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
        public List<OrderItemUIVM> Items { get; set; } = new List<OrderItemUIVM>();
    }
}
