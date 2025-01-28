using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Area.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TournamentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }

         public TournamentController(ApplicationDbContext context,
             UserManager<ApplicationUser> userManager,
             INotyfService notyfService)
        {
            _context = context;
            _notification = notyfService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var listOfTournaments = await _context.Tournaments.ToListAsync(); ;

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            

            var listOfTournamentsVM = listOfTournaments.Select(x => new TournamentVM()
            {
                Id = x.Id,
                Nome=x.Nome,
                Data=x.Data,
                LinkBando=x.LinkBando,
                LinkPreiscrizione=x.LinkPreiscrizione
            }).ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(await listOfTournamentsVM.OrderByDescending(x => x.Data).ToPagedListAsync(pageNumber, pageSize));
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTournamentVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            //get logged in user id
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var tournament = new Tournament();

            tournament.Nome = vm.Nome;
            tournament.Data= vm.Data;
            tournament.LinkBando = vm.LinkBando;
            tournament.LinkPreiscrizione= vm.LinkPreiscrizione;

            if (tournament.Nome!= null)
            {
                string slug = vm.Nome!.Trim();
                slug = slug.Replace(" ", "-");
                tournament.Url = slug + "-" + Guid.NewGuid();
            }

           await _context.Tournaments!.AddAsync(tournament);
            await _context.SaveChangesAsync();
            _notification.Success("Torneo creato");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tournament = await _context.Tournaments!.FirstOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);



            _context.Tournaments!.Remove(tournament!);
            await _context.SaveChangesAsync();
            _notification.Success("Torneo eliminato");
            return RedirectToAction("Index", "Tournament", new { area = "Admin" });


        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tournament = await _context.Tournaments!.FirstOrDefaultAsync(x => x.Id == id);
            if (tournament == null)
            {
                _notification.Error("Torneo non trovato");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);



            var vm = new TournamentVM()
            {
                Id = tournament.Id,
                Nome = tournament.Nome,
                Data = tournament.Data,
                LinkBando = tournament.LinkBando,
                LinkPreiscrizione = tournament.LinkPreiscrizione,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TournamentVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var tournament = await _context.Tournaments!.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (tournament == null)
            {
                _notification.Error("torneo non trovato");
                return View();
            }

            tournament.Nome= vm.Nome;
            tournament.Data = vm.Data;
            tournament.LinkBando = vm.LinkBando;
            tournament.LinkPreiscrizione = vm.LinkPreiscrizione;

          
            await _context.SaveChangesAsync();
            _notification.Success("Torneo aggiornato con successo");
            return RedirectToAction("Index", "Tournament", new { area = "Admin" });
        }
    }
}
