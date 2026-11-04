namespace Janz.Mvc.ViewModels.Community
{
    public class AuthorUIVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string? AvatarPath { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
