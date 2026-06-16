using SitoLtb.Models;
using System.ComponentModel.DataAnnotations;

namespace SitoLtb.ViewModels
{
    public class CreatePostVM
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ApplicationUserId { get; set; }
        [Required]
        [StringLength(4000)]
        public string? Description { get; set; }
        [Required]
        [StringLength(100)]
        public string? Categoria { get; set; }

        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
