using Microsoft.AspNetCore.Mvc;

namespace SitoLtb.Controllers
{
    public class ScuolaController : Controller
    {

        [HttpGet("redirect")]
        public IActionResult RedirectToExternalUrl()
        {
            string externalUrl = "https://lichess.org/study/9WnZoaV6/1ukJsQ0R";
            return Redirect(externalUrl);
        }
        public IActionResult BundlePrincipianti()
        {
            return View();
        }

    }
}
