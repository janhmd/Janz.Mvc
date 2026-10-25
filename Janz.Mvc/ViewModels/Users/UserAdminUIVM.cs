using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewModels.Users
{
    // Admin panel: istifadəçi (api/admin/users)
    public class UserAdminUIVM
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
        public List<OrderUIVM> Orders { get; set; } = new List<OrderUIVM>();
    }

    public class UserListVM
    {
        public List<UserAdminUIVM> Users { get; set; } = new List<UserAdminUIVM>();
        public string? Role { get; set; }
        public bool? Blocked { get; set; }
        public string? Search { get; set; }
    }

    public class RoleUIVM
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
}
