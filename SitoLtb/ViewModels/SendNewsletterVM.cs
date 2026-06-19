using System.ComponentModel.DataAnnotations;

namespace SitoLtb.ViewModels
{
    public class SendNewsletterVM
    {
        [Required(ErrorMessage = "L'oggetto è obbligatorio")]
        [MaxLength(200)]
        public string Oggetto { get; set; } = "";

        [Required(ErrorMessage = "Il corpo è obbligatorio")]
        public string Corpo { get; set; } = "";
    }
}
