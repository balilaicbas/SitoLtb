
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration=configuration;
        }

        public IActionResult Index()
        {
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];


            ViewData["ApiKey"] = googleMapsApiKey;
            return View();
        }

        public IActionResult TorneiEventi()
        {
            return View();
        }
        public IActionResult InserimentoSoci()
        {
            return View();
        }
        public IActionResult Scuola()
        {
            return View();
        }
        public IActionResult DoveGiocare()
        {
            return View();
        }

    }
}
