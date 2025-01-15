using AspNetCoreGeneratedDocument;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Rules;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.ViewModels;

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
                                INotyfService notyfService)
        {
            _context = context;
            _notification = notyfService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            
            IQueryable<Tournament> query = _context.Tournaments!
                .AsQueryable();  // Mantieni la query come IQueryable



            // Ordina la query
            var orderedQuery = query.OrderByDescending(x => x.Data);

            // Pagina la query
            var paginatedList = await orderedQuery
                .Skip((page ?? 1 - 1) * 5)  // Calcola il numero di elementi da saltare
                .Take(5)                    // Prendi i successivi 5 elementi
                .Select(x => new TournamentVM()   // Trasforma i dati in PostVM
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Data = x.Data,
                    LinkBando= x.LinkBando,
                    LinkPreiscrizione = x.LinkPreiscrizione

                })
                .ToListAsync();  // Materializza la lista

            // Creazione della vista con la lista paginata
            return View(paginatedList);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Create(TournamentVM vm)
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
