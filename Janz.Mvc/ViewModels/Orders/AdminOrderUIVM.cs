namespace Janz.Mvc.ViewModels.Orders
{
    // Admin paneldə sifariş: müştəri və keçə biləcəyi statuslar da gəlir (api/admin/orders)
    public class AdminOrderUIVM : OrderUIVM
    {
        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public List<string> NextStatuses { get; set; } = new List<string>();
    }
}
