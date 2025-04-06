
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SitoLtb.Data;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];


            ViewData["ApiKey"] = googleMapsApiKey;
            return View();
        }
        public async Task<IActionResult> Preiscrizione(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var tournaments = await _context.Tournaments!
                .OrderBy(t => t.Data)
                .Where(x => x.Data >= DateTime.Today)
                .Select(t => new SitoLtb.ViewModels.TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkBando = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                })
                .ToPagedListAsync(pageNumber, pageSize); // Converte in PagedList

            return View(tournaments); // Passa il modello corretto alla vista
        }
        public async Task<IActionResult> PreiscrizioneVerdolina(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var tournaments = await _context.Tournaments!
                .OrderBy(t => t.Data)
                .Where(x => x.Data >= DateTime.Today)
                .Where(x=> x.Tipologia=="Verdolina")
                .Select(t => new SitoLtb.ViewModels.TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkBando = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                })
                .ToPagedListAsync(pageNumber, pageSize); // Converte in PagedList

            return View(tournaments); // Passa il modello corretto alla vista
        }
        public async Task<IActionResult> PreiscrizioneComala(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var tournaments = await _context.Tournaments!
                .OrderBy(t => t.Data)
                .Where(x => x.Data >= DateTime.Today)
                .Where(x => x.Tipologia == "Comala")
                .Select(t => new SitoLtb.ViewModels.TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkBando = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                })
                .ToPagedListAsync(pageNumber, pageSize); // Converte in PagedList

            return View(tournaments); // Passa il modello corretto alla vista
        }
        public async Task<IActionResult> PreiscrizioneWeekend(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            var tournaments = await _context.Tournaments!
                .OrderBy(t => t.Data)
                .Where(x => x.Data >= DateTime.Today)
                .Where(x => x.Tipologia == "Weekend")
                .Select(t => new SitoLtb.ViewModels.TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkBando = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                })
                .ToPagedListAsync(pageNumber, pageSize); // Converte in PagedList

            return View(tournaments); // Passa il modello corretto alla vista
        }
        public IActionResult Corsi()
        {
            return View();
        }
        public async Task<IActionResult> Blog(int? page)
        {
            var vm = new BlogVM();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.Posts = await _context.Posts!
      .Include(x => x.ApplicationUser)
      .Where(x => x.Categoria == "Tornei")
      .OrderByDescending(x => x.DateTimeCreated)
      .ToPagedListAsync(pageNumber, pageSize);

            return View(vm);
        }
        public async Task<IActionResult> Universita(int? page)
        {
            var vm = new BlogVM();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.Posts = await _context.Posts!
      .Include(x => x.ApplicationUser)
      .Where(x => x.Categoria == "Universita")
      .OrderByDescending(x => x.DateTimeCreated)
      .ToPagedListAsync(pageNumber, pageSize);

            return View(vm);
        }

        public IActionResult ChiSiamo()
        {
            return View();
        }
        public IActionResult Contatti()
        {
            return View();
        }
        public IActionResult InserimentoSoci()
        {
            return View();
        }
       
        public IActionResult DoveGiocare()
        {
            return View();
        }

    }
}
