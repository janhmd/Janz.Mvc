using System.Security.Claims;

namespace Janz.Mvc.Helpers
{
    // Rol adları (API-dəki Domain/Constants/Roles.cs ilə eyni olmalıdır: MVC layihəsi Domain-i görmür, ona görə təkrar yazılıb)
    public static class AppRoles
    {
        public const string User = "User";
        public const string Support = "Support";
        public const string Editor = "Editor";
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";

        // [Authorize(Roles = ...)] üçün: admin panelə girə bilən bütün işçilər
        public const string Staff = SuperAdmin + "," + Admin + "," + Editor + "," + Support;

        // Mağazanı idarə edənlər (məhsul, kateqoriya, teq)
        public const string ShopManagers = SuperAdmin + "," + Admin;

        // Sifarişlər və istifadəçilər: Support da görür
        public const string OrderManagers = SuperAdmin + "," + Admin + "," + Support;

        private static readonly string[] StaffRoles = { SuperAdmin, Admin, Editor, Support };

        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole(Admin) || user.IsInRole(SuperAdmin);
        }

        public static bool IsStaff(ClaimsPrincipal user)
        {
            return StaffRoles.Any(user.IsInRole);
        }

        // Admin panelin yuxarısında göstərilən rol
        public static string MainRole(ClaimsPrincipal user)
        {
            return StaffRoles.FirstOrDefault(user.IsInRole) ?? User;
        }
    }
}
