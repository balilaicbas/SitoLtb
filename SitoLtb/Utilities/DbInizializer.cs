using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using SitoLtb.Models;
using SitoLtb.Data;

namespace SitoLtb.Utilities
{
    public class DbInizializer : IDbInizializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInizializer(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            

            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "balilaicbas",
                    Email = "marco.mina999@gmail.com",
                    FirstName = "Marco",
                    LastName = "Mina"
                }, "Dadomom0!").Wait();

                var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email== "marco.mina999@gmail.com");
                if (appUser != null)
                {
                    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                }       
               
            }
            _context.SaveChanges();
        }
    }
}


