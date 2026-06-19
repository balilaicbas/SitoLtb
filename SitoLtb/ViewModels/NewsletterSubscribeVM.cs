using System.ComponentModel.DataAnnotations;

namespace SitoLtb.ViewModels
{
    public class NewsletterSubscribeVM
    {
        [MaxLength(100)]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = "";
    }
}
