namespace Janz.Mvc.Helpers
{
    // Admin panelin menyusu və hansı rolların hansı bölməni gördüyü.
    // Yeni admin səhifəsi yazanda Controller dolur; boş qalanlar menyuda "soon" kimi görünür
    public class AdminSection
    {
        public string Group { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;          // bootstrap-icons adı (bi-...)
        public string? Controller { get; set; }                     // Areas/Admin-dəki controller; yoxdursa hələ yazılmayıb
        public string[] Roles { get; set; } = Array.Empty<string>();
    }

    public static class AdminSections
    {
        private static readonly string[] All = { AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.Editor, AppRoles.Support };
        private static readonly string[] Shop = { AppRoles.SuperAdmin, AppRoles.Admin };
        private static readonly string[] Orders = { AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.Support };
        private static readonly string[] Content = { AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.Editor };
        private static readonly string[] Super = { AppRoles.SuperAdmin };

        public static readonly AdminSection[] Items =
        {
            new() { Group = "Menu", Title = "Dashboard", Icon = "bi-grid-1x2", Controller = "Dashboard", Roles = All },

            new() { Group = "Shop", Title = "Products", Icon = "bi-box-seam", Controller = "Products", Roles = Shop },
            new() { Group = "Shop", Title = "Categories", Icon = "bi-layers", Controller = "Categories", Roles = Shop },
            new() { Group = "Shop", Title = "Tags", Icon = "bi-tags", Controller = "Tags", Roles = Shop },
            new() { Group = "Shop", Title = "Orders", Icon = "bi-bag-check", Controller = "Orders", Roles = Orders },
            new() { Group = "Shop", Title = "Coupons", Icon = "bi-percent", Controller = "Coupons", Roles = Shop },
            new() { Group = "Shop", Title = "Gift certificates", Icon = "bi-gift", Controller = "GiftCertificates", Roles = Shop },
            new() { Group = "Shop", Title = "Discount levels", Icon = "bi-trophy", Controller = "DiscountLevels", Roles = Shop },

            new() { Group = "Content", Title = "Journal", Icon = "bi-journal-text", Roles = Content },
            new() { Group = "Content", Title = "Community", Icon = "bi-chat-square-text", Roles = Content },
            new() { Group = "Content", Title = "Pages", Icon = "bi-file-earmark-text", Roles = Content },

            new() { Group = "Customers", Title = "Users", Icon = "bi-people", Controller = "Users", Roles = Orders },
            new() { Group = "Customers", Title = "Roles", Icon = "bi-shield-lock", Controller = "Roles", Roles = Super },
            new() { Group = "Customers", Title = "Reviews", Icon = "bi-star", Controller = "Reviews", Roles = Content },
            new() { Group = "Customers", Title = "Notifications", Icon = "bi-bell", Roles = Orders },

            new() { Group = "System", Title = "Settings", Icon = "bi-gear", Roles = Super }
        };

        // İstifadəçinin görə biləcəyi bölmələr (qrup sırası ilə)
        public static IEnumerable<AdminSection> For(System.Security.Claims.ClaimsPrincipal user)
        {
            return Items.Where(m => m.Roles.Any(user.IsInRole));
        }
    }
}
