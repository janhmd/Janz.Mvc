namespace Janz.Mvc.ViewModels.Account
{
    // API-nin login və register cavabı
    public class LoginResponseVM
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
