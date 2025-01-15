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
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                }, "Admin@0011").Wait();

                var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email== "admin@gmail.com");
                if (appUser != null)
                {
                    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                }


                var listOfPages = new List<Page>()
                {
                    new Page()
                    {
                        Title = "Prossimi Tornei",
                        Url = "Prossimi-Tornei"
                    },
                    new Page()
                    {
                        Title = "Articoli",
                        Url = "Articoli"
                    },
                    new Page()
                    {
                        Title = "Comala",
                        Url = "Serata-Comala"
                    },
                    new Page()
                    {
                        Title = "Dove si gioca a Torino",
                        Url = "Dove-Torino"
                    }

                };
                _context.Pages!.AddRange(listOfPages);
               
            }
            _context.SaveChanges();
        }
    }
}


