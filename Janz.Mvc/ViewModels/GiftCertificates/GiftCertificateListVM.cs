namespace Janz.Mvc.ViewModels.GiftCertificates
{
    public class GiftCertificateListVM
    {
        public static readonly string[] Statuses = { "Active", "Used", "Expired", "Disabled" };

        public List<GiftCertificateVM> Certificates { get; set; } = new List<GiftCertificateVM>();
        public string? Status { get; set; }
    }
}
