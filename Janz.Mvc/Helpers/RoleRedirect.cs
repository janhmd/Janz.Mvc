namespace Janz.Mvc.Helpers
{
    // Login-dən sonra hara getməli: işçi admin panelinə, müştəri ana səhifəyə
    public static class RoleRedirect
    {
        public const string AdminHome = "/Admin";
        public const string CustomerHome = "/";

        public static string HomeUrl(System.Security.Claims.ClaimsPrincipal user)
        {
            return AppRoles.IsStaff(user) ? AdminHome : CustomerHome;
        }
    }
}
