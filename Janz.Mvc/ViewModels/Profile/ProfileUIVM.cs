namespace Janz.Mvc.ViewModels.Profile
{
    // api/profile: kabinetdəki "My details"
    public class ProfileUIVM
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Nickname { get; set; }
        public string? AvatarPath { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationAddress { get; set; }
        public string? Voen { get; set; }
        public string? TaxNumber { get; set; }
    }
}
