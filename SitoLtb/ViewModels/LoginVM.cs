using Microsoft.Build.Framework;

namespace SitoLtb.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
