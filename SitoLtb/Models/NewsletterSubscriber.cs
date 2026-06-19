using System.ComponentModel.DataAnnotations;

namespace SitoLtb.Models
{
    public class NewsletterSubscriber
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string? Nome { get; set; }

        public DateTime DataIscrizione { get; set; } = DateTime.UtcNow;

        public bool Attivo { get; set; } = true;
    }
}
