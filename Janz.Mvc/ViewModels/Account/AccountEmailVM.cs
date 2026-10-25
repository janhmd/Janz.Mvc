namespace Janz.Mvc.ViewModels.Account
{
    // Təsdiq linkini yenidən göndərmək və şifrəni unutmaq formaları (API-nin {email} cavabı da)
    public class AccountEmailVM
    {
        public string Email { get; set; } = string.Empty;
    }

    // Emaildəki linkdən gələn userId və token formada gizli saxlanılır
    public class ResetPasswordVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? RepeatPassword { get; set; }
    }
}
