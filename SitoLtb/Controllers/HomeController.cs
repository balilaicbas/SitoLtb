
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SitoLtb.Data;
using SitoLtb.ViewModels;
using X.PagedList;
using SitoLtb.Services;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IPostService _postService;
        private readonly ITournamentService _tournamentService;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, ApplicationDbContext context, IPostService postService, ITournamentService tournamentService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _postService = postService;
            _tournamentService = tournamentService;

        }

        public IActionResult Index()
        {
            var vm = new IndexVM
            {
                Posts = _postService.GetAll(),
                Tournaments = _tournamentService.GetAll()
            };
           
            return View(vm);
        }

        //per il momento include solo i tornei alla Verdolina, poi sarà da aggiornare con i tornei al comala e i tornei dei bar
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
                .OrderByDescending(t => t.Data)
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
                .OrderByDescending(t => t.Data)
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
                .OrderByDescending(t => t.Data)
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
        public IActionResult Notizie(int? pageInEvidenza, int? pageTornei, int? pageEventi, int? pageCis)
        {
            int pageSize = 5;

            var vm = new BlogVM
            {
                InEvidenza = _context.Posts
                    .Where(p => p.Categoria == "In evidenza")
                    .OrderByDescending(p => p.DateTimeCreated)
                    .ToPagedList(pageInEvidenza ?? 1, pageSize),

                Tornei = _context.Posts
                    .Where(p => p.Categoria == "Tornei")
                    .OrderByDescending(p => p.DateTimeCreated)
                    .ToPagedList(pageTornei ?? 1, pageSize),

                Eventi = _context.Posts
                    .Where(p => p.Categoria == "Eventi")
                    .OrderByDescending(p => p.DateTimeCreated)
                    .ToPagedList(pageEventi ?? 1, pageSize),

                Cis = _context.Posts
                    .Where(p => p.Categoria == "Cis")
                    .OrderByDescending(p => p.DateTimeCreated)
                    .ToPagedList(pageCis ?? 1, pageSize)
            };

            return View(vm);
        }



    }
}
