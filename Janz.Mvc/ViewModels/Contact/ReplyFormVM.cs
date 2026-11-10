using System.ComponentModel.DataAnnotations;

namespace Janz.Mvc.ViewModels.Contact
{
    // Admin: müştəriyə cavab (e-poçtla gedir)
    public class ReplyFormVM
    {
        [Required(ErrorMessage = "Write the reply.")]
        [StringLength(2000, ErrorMessage = "The reply can be at most 2000 characters.")]
        public string Reply { get; set; } = string.Empty;
    }
}
