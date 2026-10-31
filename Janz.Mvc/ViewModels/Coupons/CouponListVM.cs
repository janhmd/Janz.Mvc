namespace Janz.Mvc.ViewModels.Coupons
{
    public class CouponListVM
    {
        public static readonly string[] Statuses = { "Active", "Scheduled", "Expired", "Used up", "Disabled" };

        public List<CouponVM> Coupons { get; set; } = new List<CouponVM>();
        public string? Status { get; set; }
    }
}
