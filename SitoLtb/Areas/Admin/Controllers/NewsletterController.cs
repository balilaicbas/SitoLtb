using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Services;
using SitoLtb.Utilities;
using SitoLtb.ViewModels;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRoles.WebsiteAdmin)]
    public class NewsletterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NewsletterController> _logger;

        public NewsletterController(ApplicationDbContext context, IEmailService emailService, ILogger<NewsletterController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // ── Lista iscritti ────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var iscritti = await _context.NewsletterSubscribers
                .OrderByDescending(s => s.DataIscrizione)
                .ToListAsync();
            return View(iscritti);
        }

        // ── Elimina iscritto ──────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var sub = await _context.NewsletterSubscribers.FindAsync(id);
            if (sub != null)
            {
                _context.NewsletterSubscribers.Remove(sub);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Componi e invia newsletter ────────────────────────────────────────
        [HttpGet]
        public IActionResult Invia() => View(new SendNewsletterVM());

        [HttpPost]
        public async Task<IActionResult> Invia(SendNewsletterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var destinatari = await _context.NewsletterSubscribers
                .Where(s => s.Attivo)
                .Select(s => s.Email)
                .ToListAsync();

            if (!destinatari.Any())
            {
                TempData["Err"] = "Nessun iscritto attivo a cui inviare la newsletter.";
                return View(vm);
            }

            try
            {
                await _emailService.SendNewsletterAsync(destinatari, vm.Oggetto, vm.Corpo);
                TempData["Ok"] = $"Newsletter inviata a {destinatari.Count} iscritti.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore invio newsletter");
                TempData["Err"] = "Errore durante l'invio. Controlla la configurazione SMTP.";
                return View(vm);
            }
        }
    }
}
