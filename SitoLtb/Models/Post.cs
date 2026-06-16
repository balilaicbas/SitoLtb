
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SitoLtb.Data;

namespace SitoLtb.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public string? ApplicationUserId { get; set; }

        public string? Image { get; set; }

        [Required]
        [StringLength(300)]
        public string Url { get; set; }

        [Required]
        [StringLength(100)]
        public string Categoria { get; set; }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
