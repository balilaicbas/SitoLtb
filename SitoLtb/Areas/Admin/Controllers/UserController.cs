using AspNetCoreHero.ToastNotification.Abstractions;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Utilities;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserController> _logger;
        public INotyfService _notification { get; }
        public UserController(UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    INotyfService notyfService,
                                    ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notyfService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var vm = users.Select(x => new UserVM()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();
            //assinging roles
            foreach(var user in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(singleUser);
                user.Roles = roles;
            }

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                _notification.Error("Utente non trovato");
                return RedirectToAction(nameof(Index));
            }
            var roles = await _userManager.GetRolesAsync(existingUser);
            var vm = new EditRolesVM()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName,
                IsAdmin = roles.Contains(WebsiteRoles.WebsiteAdmin),
                IsEditor = roles.Contains(WebsiteRoles.Editor),
                IsData = roles.Contains(WebsiteRoles.Data)
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRolesVM vm)
        {
            var existingUser = await _userManager.FindByIdAsync(vm.Id);
            if (existingUser == null)
            {
                _notification.Error("Utente non trovato");
                return RedirectToAction(nameof(Index));
            }

            var desiredRoles = new List<string>();
            if (vm.IsAdmin) desiredRoles.Add(WebsiteRoles.WebsiteAdmin);
            if (vm.IsEditor) desiredRoles.Add(WebsiteRoles.Editor);
            if (vm.IsData) desiredRoles.Add(WebsiteRoles.Data);

            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
            if (desiredRoles.Count > 0)
            {
                await _userManager.AddToRolesAsync(existingUser, desiredRoles);
            }

            _notification.Success("Ruoli aggiornati con successo");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                _notification.Error("User doesnot exsits");
                return View();
            }
            var vm = new ResetPasswordVM()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid){ return View(vm); }
            var existingUser = await _userManager.FindByIdAsync(vm.Id);
            if (existingUser == null)
            {
                _notification.Error("User doesnot exist");
                return View(vm);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var result = await _userManager.ResetPasswordAsync(existingUser, token, vm.NewPassword!);
            if (result.Succeeded)
            {
                _notification.Success("Password reset succuful");
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }


        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (checkUserByEmail != null)
            {
                _notification.Error("Email already exists");
                return View(vm);
            }
            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
            if (checkUserByUsername != null)
            {
                _notification.Error("Username already exists");
                return View(vm);
            }

            var applicationUser = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName
            };

            var result =  await _userManager.CreateAsync(applicationUser,vm.Password);

            if (result.Succeeded)
            {
                var rolesToAssign = new List<string>();
                if (vm.IsAdmin) rolesToAssign.Add(WebsiteRoles.WebsiteAdmin);
                if (vm.IsEditor) rolesToAssign.Add(WebsiteRoles.Editor);
                if (vm.IsData) rolesToAssign.Add(WebsiteRoles.Data);

                if (rolesToAssign.Count > 0)
                {
                    await _userManager.AddToRolesAsync(applicationUser, rolesToAssign);
                }

                _notification.Success("User registered successfully");
                return RedirectToAction("Index", "User", new {area="Admin"});
            }
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users!.FirstOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);


            var result = await _userManager.DeleteAsync(user!);
            if (result.Succeeded)
            {
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            else
            {
                _notification.Error("Non è stato possibile rimuovere l'utente, rimuovere direttamente a db");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }


        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, isPersistent: true, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _notification.Success("Login Successful");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Account {Username} bloccato temporaneamente per troppi tentativi di login falliti.", vm.Username);
                _notification.Error("Account temporaneamente bloccato per troppi tentativi falliti. Riprova più tardi.");
                return View(vm);
            }

            _notification.Error("Username o password non corretti");
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("You are logged out successfully");
            return RedirectToAction("Index", "Home" , new {area = ""});
        }

        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
