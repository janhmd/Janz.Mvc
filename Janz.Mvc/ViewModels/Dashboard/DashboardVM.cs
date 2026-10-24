namespace Janz.Mvc.ViewModels.Dashboard
{
    // Admin panelin ana səhifəsi: API-dəki api/admin/dashboard cavabı
    public class DashboardVM
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int OrderCount { get; set; }
        public int OrdersToday { get; set; }
        public int PendingOrders { get; set; }
        public decimal AverageOrder { get; set; }
        public int CustomerCount { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ProductCount { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();
        public List<DailySalesVM> SalesLast7Days { get; set; } = new List<DailySalesVM>();
        public List<TopProductVM> TopProducts { get; set; } = new List<TopProductVM>();
        public List<LowStockVM> LowStock { get; set; } = new List<LowStockVM>();
        public List<RecentOrderVM> RecentOrders { get; set; } = new List<RecentOrderVM>();
    }

    public class DailySalesVM
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int Orders { get; set; }
    }

    public class TopProductVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LowStockVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
    }

    public class RecentOrderVM
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
