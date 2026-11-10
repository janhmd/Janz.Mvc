using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.GiftCertificates
{
    // Admin: yeni hədiyyə sertifikatı. Müştəri kodu kabinetdə daxil edəndə məbləğ balansına düşür
    public class GiftCertificateCreateVM
    {
        [Required(ErrorMessage = "Enter the code.")]
        [StringLength(20, ErrorMessage = "The code can be at most 20 characters.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "The code can have only latin letters and digits.")]
        public string? Code { get; set; }

        [Range(0.01, 5000, ErrorMessage = "The amount must be between 0.01 and 5000.")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Sertifikatın kodu seçilən müştərilərin bildirişlərinə göndərilsin
        public Janz.Mvc.ViewModels.Coupons.CouponAudienceVM Audience { get; set; } = new Janz.Mvc.ViewModels.Coupons.CouponAudienceVM();
    }
}
