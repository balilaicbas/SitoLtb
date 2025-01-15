using Microsoft.AspNetCore.Mvc;

namespace SitoLtb.Area.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
