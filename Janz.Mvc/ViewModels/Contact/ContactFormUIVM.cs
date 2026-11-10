using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Contact
{
    // Contact səhifəsindəki forma (api/contact). Mövzular API-dəki Domain/Constants/ContactTopics.cs ilə eyni olmalıdır
    public class ContactFormUIVM
    {
        public static readonly string[] Topics = { "Order", "Payment", "Product", "Complaint", "Other" };

        [Required(ErrorMessage = "Enter your name.")]
        [StringLength(80, ErrorMessage = "The name can be at most 80 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        [StringLength(150, ErrorMessage = "The email can be at most 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose what your message is about.")]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Write your message.")]
        [MinLength(10, ErrorMessage = "The message is too short. Please add a few more details.")]
        [StringLength(2000, ErrorMessage = "The message can be at most 2000 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}
