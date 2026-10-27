namespace Janz.Mvc.ViewModels.Users
{
    public class UserListVM
    {
        public List<UserAdminVM> Users { get; set; } = new List<UserAdminVM>();
        public string? Role { get; set; }
        public bool? Blocked { get; set; }
        public string? Search { get; set; }
    }
}
