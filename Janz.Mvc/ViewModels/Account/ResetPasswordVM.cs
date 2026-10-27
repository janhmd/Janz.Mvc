namespace Janz.Mvc.ViewModels.Account
{
    // Emaildəki linkdən gələn userId və token formada gizli saxlanılır
    public class ResetPasswordVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? RepeatPassword { get; set; }
    }
}
