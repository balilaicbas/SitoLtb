using System.ComponentModel.DataAnnotations;

namespace SitoLtb.Models
{
    public class Tournament
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        [Required]
        [StringLength(100)]
        public string Tipologia { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Sede { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LinkBando { get; set; }

        [StringLength(500)]
        public string? LinkPreiscrizione { get; set; }

        [StringLength(300)]
        public string? Url { get; set; }

        public bool Elo { get; set; } = false;
    }
}