using System.ComponentModel.DataAnnotations;

namespace SitoLtb.ViewModels
{
    public class ResetPasswordVM
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
