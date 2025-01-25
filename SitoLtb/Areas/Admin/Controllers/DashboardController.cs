using Microsoft.AspNetCore.Mvc;

namespace SitoLtb.Area.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
