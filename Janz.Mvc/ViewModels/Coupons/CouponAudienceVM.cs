using Janz.Mvc.ViewModels.Users;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Janz.Mvc.ViewModels.Coupons
{
    // Admin: kupon kimlərə göndərilir. Mode: none | all | selected (seçilmişlərin UserIds-i)
    public class CouponAudienceVM
    {
        public string Mode { get; set; } = "none";
        public List<string> UserIds { get; set; } = new List<string>();

        // Seçim siyahısı üçün API-dən alınır, formadan oxunmur
        [BindNever]
        public List<UserAdminVM> Customers { get; set; } = new List<UserAdminVM>();
    }
}
