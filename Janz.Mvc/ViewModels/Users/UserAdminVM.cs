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

    public class UserListVM
    {
        public List<UserAdminVM> Users { get; set; } = new List<UserAdminVM>();
        public string? Role { get; set; }
        public bool? Blocked { get; set; }
        public string? Search { get; set; }
    }

    public class RoleVM
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
}
