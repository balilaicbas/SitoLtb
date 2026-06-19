using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SitoLtb.Services;
using SitoLtb.Utilities;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRoles.WebsiteAdmin)]
    public class ArchiveController : Controller
    {
        private readonly IArchiveService _archiveService;

        public ArchiveController(IArchiveService archiveService)
        {
            _archiveService = archiveService;
        }

        public async Task<IActionResult> Index(int? pageEventi, int? pageArticoli)
        {
            var archiveVM = await _archiveService.GetArchiveAsync(pageEventi ?? 1, pageArticoli ?? 1);
            return View(archiveVM);
        }
    }
}