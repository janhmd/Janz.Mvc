using System.Security.Claims;

namespace Janz.Mvc.Helpers
{
    // Rol adları (API-dəki Domain/Constants/Roles.cs ilə eyni olmalıdır: MVC layihəsi Domain-i görmür, ona görə təkrar yazılıb)
    public static class AppRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole(Admin);
        }
    }
}
