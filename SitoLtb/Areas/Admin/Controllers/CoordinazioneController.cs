using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.Utilities;
using SitoLtb.ViewModels;

namespace SitoLtb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{WebsiteRoles.WebsiteAdmin},{WebsiteRoles.Editor}")]
public class CoordinazioneController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoordinazioneController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<string?> NomeUtente(string? id)
    {
        if (id == null) return null;
        var u = await _userManager.FindByIdAsync(id);
        return u == null ? null : $"{u.FirstName} {u.LastName}".Trim();
    }

    private async Task<List<MembroVM>> TuttiGliUtenti()
    {
        var users = _userManager.Users.ToList();
        return users.Select(u => new MembroVM
        {
            UserId = u.Id,
            Nome   = $"{u.FirstName} {u.LastName}".Trim(),
            Email  = u.Email ?? ""
        }).OrderBy(u => u.Nome).ToList();
    }

    private async Task<ProgettoTaskVM> MapTask(ProgettoTask t)
    {
        var commenti = new List<TaskCommentoVM>();
        foreach (var c in t.Commenti.OrderBy(c => c.DataCreazione))
        {
            commenti.Add(new TaskCommentoVM
            {
                IdCommento    = c.IdCommento,
                AutoreNome    = await NomeUtente(c.AutoreId) ?? "—",
                Testo         = c.Testo,
                DataCreazione = c.DataCreazione
            });
        }
        return new ProgettoTaskVM
        {
            IdTask            = t.IdTask,
            IdProgetto        = t.IdProgetto,
            Titolo            = t.Titolo,
            Descrizione       = t.Descrizione,
            Stato             = t.Stato,
            Priorita          = t.Priorita,
            DataScadenza      = t.DataScadenza,
            AssegnatoAId      = t.AssegnatoAId,
            AssegnatoANome    = await NomeUtente(t.AssegnatoAId),
            DataCreazione     = t.DataCreazione,
            DataCompletamento = t.DataCompletamento,
            Commenti          = commenti
        };
    }

    // ── Index ─────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Index()
    {
        var progetti = await _db.Progetti
            .Include(p => p.Tasks)
            .Include(p => p.Membri)
            .Include(p => p.Scadenze)
            .Include(p => p.Note)
            .OrderByDescending(p => p.DataCreazione)
            .ToListAsync();

        var cards = new List<ProgettoCardVM>();
        foreach (var p in progetti)
        {
            cards.Add(new ProgettoCardVM
            {
                IdProgetto    = p.IdProgetto,
                Titolo        = p.Titolo,
                Descrizione   = p.Descrizione,
                DataInizio    = p.DataInizio,
                DataScadenza  = p.DataScadenza,
                Colore        = p.Colore,
                Stato         = p.Stato,
                ReferenteNome = await NomeUtente(p.ReferenteId),
                TotaleTask    = p.Tasks.Count,
                TaskRisolte   = p.Tasks.Count(t => t.Stato == StatoTask.Risolto),
                NumMembri     = p.Membri.Count,
                NumScadenze   = p.Scadenze.Count,
                NumNote       = p.Note.Count
            });
        }

        var vm = new CoordinazioneIndexVM
        {
            Progetti          = cards,
            TotaleProgetti    = cards.Count,
            ProgettiAttivi    = cards.Count(c => c.Stato == StatoProgetto.Attivo),
            ProgettiCompletati = cards.Count(c => c.Stato == StatoProgetto.Completato),
            ProgettiSospesi   = cards.Count(c => c.Stato == StatoProgetto.Sospeso)
        };
        return View(vm);
    }

    // ── Dettaglio ─────────────────────────────────────────────────────────────

    public async Task<IActionResult> Dettaglio(int id)
    {
        var p = await _db.Progetti
            .Include(p => p.Tasks).ThenInclude(t => t.Commenti)
            .Include(p => p.Note)
            .Include(p => p.Scadenze)
            .Include(p => p.Membri)
            .FirstOrDefaultAsync(p => p.IdProgetto == id);

        if (p == null) return NotFound();

        var tasks = new List<ProgettoTaskVM>();
        foreach (var t in p.Tasks.OrderBy(t => t.DataCreazione))
            tasks.Add(await MapTask(t));

        var membri = new List<MembroVM>();
        foreach (var m in p.Membri)
        {
            var u = await _userManager.FindByIdAsync(m.UserId);
            if (u != null)
                membri.Add(new MembroVM { UserId = u.Id, Nome = $"{u.FirstName} {u.LastName}".Trim(), Email = u.Email ?? "" });
        }

        var membroIds = p.Membri.Select(m => m.UserId).ToHashSet();
        var disponibili = (await TuttiGliUtenti()).Where(u => !membroIds.Contains(u.UserId)).ToList();

        var vm = new ProgettoDetailVM
        {
            IdProgetto        = p.IdProgetto,
            Titolo            = p.Titolo,
            Descrizione       = p.Descrizione,
            DataInizio        = p.DataInizio,
            DataScadenza      = p.DataScadenza,
            Colore            = p.Colore,
            Stato             = p.Stato,
            ReferenteId       = p.ReferenteId,
            ReferenteNome     = await NomeUtente(p.ReferenteId),
            Tasks             = tasks,
            Note              = p.Note.OrderByDescending(n => n.DataCreazione).ToList(),
            Scadenze          = p.Scadenze.OrderBy(s => s.Data).ToList(),
            Membri            = membri,
            UtentiDisponibili = disponibili
        };
        return View(vm);
    }

    // ── Crea ──────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Crea()
    {
        var vm = new CreaProgettoVM { UtentiDisponibili = await TuttiGliUtenti() };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Crea(CreaProgettoVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.UtentiDisponibili = await TuttiGliUtenti();
            return View(vm);
        }
        var p = new Progetto
        {
            Titolo       = vm.Titolo,
            Descrizione  = vm.Descrizione,
            DataInizio   = vm.DataInizio,
            DataScadenza = vm.DataScadenza,
            ReferenteId  = vm.ReferenteId,
            Colore       = vm.Colore,
            Stato        = vm.Stato,
            DataCreazione = DateTime.UtcNow
        };
        _db.Progetti.Add(p);
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id = p.IdProgetto });
    }

    // ── Modifica ──────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Modifica(int id)
    {
        var p = await _db.Progetti.FindAsync(id);
        if (p == null) return NotFound();
        var vm = new CreaProgettoVM
        {
            Titolo            = p.Titolo,
            Descrizione       = p.Descrizione,
            DataInizio        = p.DataInizio,
            DataScadenza      = p.DataScadenza,
            ReferenteId       = p.ReferenteId,
            Colore            = p.Colore,
            Stato             = p.Stato,
            UtentiDisponibili = await TuttiGliUtenti()
        };
        ViewBag.IdProgetto = id;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Modifica(int id, CreaProgettoVM vm)
    {
        var p = await _db.Progetti.FindAsync(id);
        if (p == null) return NotFound();
        if (!ModelState.IsValid)
        {
            vm.UtentiDisponibili = await TuttiGliUtenti();
            ViewBag.IdProgetto = id;
            return View(vm);
        }
        p.Titolo       = vm.Titolo;
        p.Descrizione  = vm.Descrizione;
        p.DataInizio   = vm.DataInizio;
        p.DataScadenza = vm.DataScadenza;
        p.ReferenteId  = vm.ReferenteId;
        p.Colore       = vm.Colore;
        p.Stato        = vm.Stato;
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id });
    }

    // ── Elimina Progetto ──────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> EliminaProgetto(int id)
    {
        var p = await _db.Progetti.FindAsync(id);
        if (p != null) { _db.Progetti.Remove(p); await _db.SaveChangesAsync(); }
        return RedirectToAction("Index");
    }

    // ── Task ──────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> NuovaTask(int idProgetto, string titolo, string? descrizione,
        PrioritaTask priorita, DateTime? dataScadenza, string? assegnatoAId)
    {
        var t = new ProgettoTask
        {
            IdProgetto   = idProgetto,
            Titolo       = titolo,
            Descrizione  = descrizione,
            Priorita     = priorita,
            DataScadenza = dataScadenza,
            AssegnatoAId = assegnatoAId,
            Stato        = StatoTask.InCorso,
            DataCreazione = DateTime.UtcNow
        };
        _db.ProgettoTasks.Add(t);
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleTask(int idTask, int idProgetto)
    {
        var t = await _db.ProgettoTasks.FindAsync(idTask);
        if (t != null)
        {
            if (t.Stato == StatoTask.InCorso)
            {
                t.Stato = StatoTask.Risolto;
                t.DataCompletamento = DateTime.UtcNow;
            }
            else
            {
                t.Stato = StatoTask.InCorso;
                t.DataCompletamento = null;
            }
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    [HttpPost]
    public async Task<IActionResult> EliminaTask(int idTask, int idProgetto)
    {
        var t = await _db.ProgettoTasks.FindAsync(idTask);
        if (t != null) { _db.ProgettoTasks.Remove(t); await _db.SaveChangesAsync(); }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    // ── Commenti ──────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> NuovoCommento(int idTask, int idProgetto, string testo)
    {
        var userId = _userManager.GetUserId(User);
        _db.TaskCommenti.Add(new TaskCommento
        {
            IdTask        = idTask,
            AutoreId      = userId,
            Testo         = testo,
            DataCreazione = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    // ── Note ──────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> NuovaNota(int idProgetto, string testo)
    {
        var userId = _userManager.GetUserId(User);
        _db.ProgettoNote.Add(new ProgettoNota
        {
            IdProgetto    = idProgetto,
            AutoreId      = userId,
            Testo         = testo,
            DataCreazione = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    [HttpPost]
    public async Task<IActionResult> EliminaNota(int idNota, int idProgetto)
    {
        var n = await _db.ProgettoNote.FindAsync(idNota);
        if (n != null) { _db.ProgettoNote.Remove(n); await _db.SaveChangesAsync(); }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    // ── Scadenze ──────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> NuovaScadenza(int idProgetto, string titolo, DateOnly data, string? nota)
    {
        _db.ProgettoScadenze.Add(new ProgettoScadenza
        {
            IdProgetto = idProgetto,
            Titolo     = titolo,
            Data       = data,
            Nota       = nota
        });
        await _db.SaveChangesAsync();
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    [HttpPost]
    public async Task<IActionResult> EliminaScadenza(int idScadenza, int idProgetto)
    {
        var s = await _db.ProgettoScadenze.FindAsync(idScadenza);
        if (s != null) { _db.ProgettoScadenze.Remove(s); await _db.SaveChangesAsync(); }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    // ── Membri ────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> AggiungiMembro(int idProgetto, string userId)
    {
        var exists = await _db.ProgettoMembri.AnyAsync(m => m.IdProgetto == idProgetto && m.UserId == userId);
        if (!exists)
        {
            _db.ProgettoMembri.Add(new ProgettoMembro { IdProgetto = idProgetto, UserId = userId });
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }

    [HttpPost]
    public async Task<IActionResult> RimuoviMembro(int idProgetto, string userId)
    {
        var m = await _db.ProgettoMembri.FindAsync(idProgetto, userId);
        if (m != null) { _db.ProgettoMembri.Remove(m); await _db.SaveChangesAsync(); }
        return RedirectToAction("Dettaglio", new { id = idProgetto });
    }
}
