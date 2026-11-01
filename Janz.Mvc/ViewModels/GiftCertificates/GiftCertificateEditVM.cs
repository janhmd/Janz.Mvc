using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.GiftCertificates
{
    // Admin: sertifikatı dəyişmək. İstifadə olunubsa kod və məbləğ dəyişmir (API yoxlayır)
    public class GiftCertificateEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the code.")]
        [StringLength(20, ErrorMessage = "The code can be at most 20 characters.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "The code can have only latin letters and digits.")]
        public string? Code { get; set; }

        [Range(0.01, 5000, ErrorMessage = "The amount must be between 0.01 and 5000.")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Yalnız göstərmək üçündür, formadan oxunmur
        [BindNever]
        public string? UsedByEmail { get; set; }

        [BindNever]
        public DateTime? UsedAt { get; set; }
    }
}
