using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SitoLtb.Models;

namespace SitoLtb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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


    }
}
