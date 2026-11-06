namespace Janz.Mvc.ViewModels.Account
{
    // Qeydiyyatda emailə gələn 4 rəqəmli kodun təsdiqi
    public class VerifyVM
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
