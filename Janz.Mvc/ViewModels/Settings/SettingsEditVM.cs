using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Settings
{
    public class SettingsEditVM
    {
        [Required(ErrorMessage = "Enter the phone.")]
        [StringLength(30, ErrorMessage = "The phone can be at most 30 characters.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        [StringLength(100, ErrorMessage = "The email can be at most 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the address.")]
        [StringLength(150, ErrorMessage = "The address can be at most 150 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the Instagram link.")]
        [StringLength(200, ErrorMessage = "The Instagram link can be at most 200 characters.")]
        public string Instagram { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the WhatsApp link.")]
        [StringLength(200, ErrorMessage = "The WhatsApp link can be at most 200 characters.")]
        public string WhatsApp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the working hours.")]
        [StringLength(100, ErrorMessage = "The working hours can be at most 100 characters.")]
        public string Hours { get; set; } = string.Empty;
    }
}
