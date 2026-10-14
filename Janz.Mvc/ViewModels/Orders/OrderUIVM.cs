namespace Janz.Mvc.ViewModels.Orders
{
    public class OrderUIVM
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemUIVM> Items { get; set; } = new List<OrderItemUIVM>();
    }
}
