using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SitoLtb.Utilities;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRoles.WebsiteAdmin + "," + WebsiteRoles.Data)]
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
