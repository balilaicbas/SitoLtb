using Microsoft.AspNetCore.Mvc;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.Services;
using SitoLtb.ViewModels;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly ITournamentService _tournamentService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            IPostService postService,
            ITournamentService tournamentService,
            IEmailService emailService,
            ApplicationDbContext context)
        {
            _logger = logger;
            _postService = postService;
            _tournamentService = tournamentService;
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new IndexVM
            {
                PostsFuturi        = _postService.GetAll(),
                TournamentsFuturi  = _tournamentService.GetAll(),
                TorneiProssimoMese = _tournamentService.GetNextMonth(),
            };
            return View(vm);
        }

        // ── Tornei / eventi ──────────────────────────────────────────────────
        public async Task<IActionResult> Preiscrizione(int? pageVerdolina, int? pageComala)
        {
            var vm = new PreiscrizioneVM
            {
                Verdolina = await _tournamentService.GetUpcomingBySedePagedAsync("Verdolina", pageVerdolina ?? 1, 4),
                Comala    = await _tournamentService.GetUpcomingBySedePagedAsync("Comala",    pageComala    ?? 1, 4)
            };
            return View(vm);
        }

        public IActionResult Calendario()
        {
            var vm = new CalendarioVM { Tournaments = _tournamentService.GetAllForCalendar() };
            return View(vm);
        }
        public IActionResult GrandeSlamBar() => View();

        // ── Corsi ────────────────────────────────────────────────────────────
        public IActionResult Scuola3Livello() => View();
        public IActionResult IstruttoriLtb()  => View();
        public IActionResult CAT()            => View();

        // ── Notizie ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Notizie(int? pageInEvidenza, int? pageTornei, int? pageEventi, int? pageCis)
        {
            const int pageSize = 5;
            var vm = new BlogVM
            {
                InEvidenza = await _postService.GetByCategoriaPagedAsync("In evidenza", pageInEvidenza ?? 1, pageSize),
                Tornei     = await _postService.GetByCategoriaPagedAsync("Tornei",      pageTornei    ?? 1, pageSize),
                Eventi     = await _postService.GetByCategoriaPagedAsync("Eventi",      pageEventi    ?? 1, pageSize),
                Cis        = await _postService.GetByCategoriaPagedAsync("Cis",         pageCis       ?? 1, pageSize)
            };
            return View(vm);
        }

        public async Task<IActionResult> NotizieInEvidenza(int? page) =>
            View("NotiziaCat", new CategoriaPostVM { Titolo = "In evidenza", Posts = await _postService.GetByCategoriaPagedAsync("In evidenza", page ?? 1, 12) });

        public async Task<IActionResult> NotizieTornei(int? page) =>
            View("NotiziaCat", new CategoriaPostVM { Titolo = "Tornei", Posts = await _postService.GetByCategoriaPagedAsync("Tornei", page ?? 1, 12) });

        public async Task<IActionResult> NotizieEventi(int? page) =>
            View("NotiziaCat", new CategoriaPostVM { Titolo = "Eventi", Posts = await _postService.GetByCategoriaPagedAsync("Eventi", page ?? 1, 12) });

        public async Task<IActionResult> NotizieCis(int? page) =>
            View("NotiziaCat", new CategoriaPostVM { Titolo = "CIS", Posts = await _postService.GetByCategoriaPagedAsync("Cis", page ?? 1, 12) });

        // ── Chi siamo ────────────────────────────────────────────────────────
        public IActionResult ChiSiamo()      => View();
        public IActionResult Iscrizione()    => View();
        public IActionResult Statuto()       => View();
        public IActionResult DoveSiamo()     => View();
        public IActionResult ParlanoDiNoi()  => View();
        public IActionResult CinquePerMille() => View();
        public IActionResult DoveGiocare()   => View();

        // ── Contatti ─────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Contatti() => View(new ContattiVM());

        [HttpPost]
        public async Task<IActionResult> Contatti(ContattiVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _emailService.SendContactAsync(vm.Nome, vm.Email, vm.Messaggio);
                TempData["ContattiOk"] = "Messaggio inviato! Ti risponderemo al più presto.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore invio email contatti");
                TempData["ContattiErr"] = "Errore nell'invio. Prova a contattarci direttamente via email.";
            }

            return RedirectToAction(nameof(Contatti));
        }

        // ── Newsletter ───────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> NewsletterIscriviti(NewsletterSubscribeVM vm, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["NewsletterErr"] = "Email non valida.";
                return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action(nameof(Contatti))!);
            }

            var exists = _context.NewsletterSubscribers.Any(s => s.Email == vm.Email);
            if (exists)
            {
                TempData["NewsletterOk"] = "Sei già iscritto alla newsletter!";
                return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action(nameof(Contatti))!);
            }

            _context.NewsletterSubscribers.Add(new NewsletterSubscriber { Email = vm.Email, Nome = vm.Nome });
            await _context.SaveChangesAsync();

            TempData["NewsletterOk"] = "Iscrizione effettuata! Grazie.";
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action(nameof(Contatti))!);
        }

        // ── Error ────────────────────────────────────────────────────────────
        [Route("/Home/Error")]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
