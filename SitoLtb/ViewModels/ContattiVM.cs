using System.ComponentModel.DataAnnotations;

namespace SitoLtb.ViewModels
{
    public class ContattiVM
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Il messaggio è obbligatorio")]
        [MaxLength(3000)]
        public string Messaggio { get; set; } = "";
    }
}
