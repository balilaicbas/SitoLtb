
using Microsoft.AspNetCore.Mvc;
using SitoLtb.ViewModels;
using SitoLtb.Services;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPostService _postService;
        private readonly ITournamentService _tournamentService;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, IPostService postService, ITournamentService tournamentService)
        {
            _logger = logger;
            _configuration = configuration;
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

        //tornei ed eventi
        public async Task<IActionResult> Preiscrizione(int? pageVerdolina, int? pageComala)
        {
            int pageSize = 4;

            int verdolinaPage = pageVerdolina ?? 1;
            int comalaPage = pageComala ?? 1;

            var vm = new SitoLtb.ViewModels.PreiscrizioneVM
            {
                Verdolina = await _tournamentService.GetUpcomingBySedePagedAsync("Verdolina", verdolinaPage, pageSize),
                Comala = await _tournamentService.GetUpcomingBySedePagedAsync("Comala", comalaPage, pageSize)
            };

            return View(vm);
        }


        public IActionResult Calendario()
        {
            return View();
        }
        public IActionResult GrandeSlamBar()
        {
            return View();
        }
        // scuola di scacchi LTB
        public IActionResult Scuola3Livello()
        {
            return View();
        }
        public IActionResult CorsoInterno()
        {
            return View();
        }
        public IActionResult CAT()
        {
            return View();
        }
        public IActionResult Offerta()
        {
            return View();
        }
        //blog
        public async Task<IActionResult> Notizie(int? pageInEvidenza, int? pageTornei, int? pageEventi, int? pageCis)
        {
            int pageSize = 5;

            var vm = new BlogVM
            {
                InEvidenza = await _postService.GetByCategoriaPagedAsync("In evidenza", pageInEvidenza ?? 1, pageSize),
                Tornei = await _postService.GetByCategoriaPagedAsync("Tornei", pageTornei ?? 1, pageSize),
                Eventi = await _postService.GetByCategoriaPagedAsync("Eventi", pageEventi ?? 1, pageSize),
                Cis = await _postService.GetByCategoriaPagedAsync("Cis", pageCis ?? 1, pageSize)
            };

            return View(vm);
        }
        public IActionResult NotizieInEvidenza()
        {
            return View();
        }
        public IActionResult NotizieTornei()
        {
            return View();
        }
        public IActionResult NotizieEventi()
        {
            return View();
        }
        public IActionResult NotizieCis()
        {
            return View();
        }

        //chi siamo
        public IActionResult ChiSiamo()
        {
            return View();
        }
        public IActionResult Iscrizione()
        {
            return View();
        }
        public IActionResult DoveSiamo()
        {
            return View();
        }
        public IActionResult ParlanoDiNoi()
        {
            return View();
        }
        //contatti
        public IActionResult Contatti()
        {
            return View();
        }

       //dove giocare a torino
        public IActionResult DoveGiocare()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.Route("/Home/Error")]
        public IActionResult Error()
        {
            return View(new SitoLtb.Models.ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }


    }
}
