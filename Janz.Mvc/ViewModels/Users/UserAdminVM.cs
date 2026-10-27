using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewModels.Users
{
    // Admin panel: istifadəçi (api/admin/users)
    public class UserAdminVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }

        // Yalnız detal səhifəsində gəlir
        public List<AdminOrderVM> Orders { get; set; } = new List<AdminOrderVM>();
    }
}
