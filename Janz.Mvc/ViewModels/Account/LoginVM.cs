namespace Janz.Mvc.ViewModels.Account
{
    public class LoginVM
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Remember { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
