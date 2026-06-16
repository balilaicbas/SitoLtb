using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SitoLtb.Models;
using SitoLtb.Data;

namespace SitoLtb.Utilities
{
    public class DbInizializer : IDbInizializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbInizializer> _logger;
        public DbInizializer(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IConfiguration configuration,
                            ILogger<DbInizializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }
        public void Initialize()
        {
            EnsureRolesExist();
            EnsureAdminUserExists();
            _context.SaveChanges();
        }

        private void EnsureRolesExist()
        {
            foreach (var role in WebsiteRoles.All)
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
            }
        }

        private void EnsureAdminUserExists()
        {
            var adminUserName = _configuration["AdminUser:UserName"];
            var adminEmail = _configuration["AdminUser:Email"];
            var adminPassword = _configuration["AdminUser:Password"];
            var adminFirstName = _configuration["AdminUser:FirstName"];
            var adminLastName = _configuration["AdminUser:LastName"];

            if (string.IsNullOrWhiteSpace(adminUserName) || string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                _logger.LogWarning("Credenziali admin (AdminUser:UserName/Email/Password) non configurate: salto la creazione dell'utente admin. Configurale con 'dotnet user-secrets' in locale o come Application Settings in produzione.");
                return;
            }

            if (_context.ApplicationUsers!.Any(x => x.Email == adminEmail))
            {
                return;
            }

            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = adminUserName,
                Email = adminEmail,
                FirstName = adminFirstName,
                LastName = adminLastName
            }, adminPassword).Wait();

            var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email == adminEmail);
            if (appUser != null)
            {
                _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
            }
        }
    }
}


