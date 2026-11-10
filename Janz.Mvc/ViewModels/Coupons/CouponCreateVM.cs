using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Coupons
{
    // Admin: yeni kupon. Type: Percent və ya Fixed
    public class CouponCreateVM
    {
        [Required(ErrorMessage = "Enter the code.")]
        [StringLength(20, ErrorMessage = "The code can be at most 20 characters.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "The code can have only latin letters and digits.")]
        public string? Code { get; set; }

        public string Type { get; set; } = "Percent";

        [Range(0.01, 10000, ErrorMessage = "The value must be between 0.01 and 10 000.")]
        public decimal Value { get; set; }

        [Range(0, 100000, ErrorMessage = "The min order must be between 0 and 100 000.")]
        public decimal MinOrder { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The usage limit must be at least 1.")]
        public int? UsageLimit { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartsAt { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndsAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Saytda kupon kartında yalnız sifarişi olmayan müştəriyə göstərilir
        public bool IsFirstOrder { get; set; }

        // Kuponun kodu seçilən müştərilərin bildirişlərinə göndərilsin
        public CouponAudienceVM Audience { get; set; } = new CouponAudienceVM();
    }
}
