using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.Services;
using SitoLtb.Utilities;
using SitoLtb.ViewModels;
using System.Text;

namespace SitoLtb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{WebsiteRoles.WebsiteAdmin},{WebsiteRoles.Editor}")]
public class CoordinazioneController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CoordinazioneDigestService _digest;
    private readonly IEmailService _email;

    public CoordinazioneController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
        CoordinazioneDigestService digest, IEmailService email)
    {
        _db = db;
        _userManager = userManager;
        _digest = digest;
        _email = email;
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

    // ── ICS helpers ──────────────────────────────────────────────────────────

    private static string BuildIcs(Progetto p)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//LTB//Coordinazione//IT");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        foreach (var s in p.Scadenze.OrderBy(s => s.Data))
        {
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:ltb-scad-{s.IdScadenza}@ltb");
            sb.AppendLine($"DTSTART;VALUE=DATE:{s.Data:yyyyMMdd}");
            sb.AppendLine($"DTEND;VALUE=DATE:{s.Data.AddDays(1):yyyyMMdd}");
            sb.AppendLine($"SUMMARY:{EscapeIcs(s.Titolo)} [{EscapeIcs(p.Titolo)}]");
            if (!string.IsNullOrEmpty(s.Nota))
                sb.AppendLine($"DESCRIPTION:{EscapeIcs(s.Nota)}");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine("END:VEVENT");
        }
        sb.AppendLine("END:VCALENDAR");
        return sb.ToString();
    }

    private async Task InviaEmailCalendario(IEmailService emailSvc, Progetto p, ApplicationUser dest, string motivazione)
    {
        if (dest.Email == null) return;
        var nome    = $"{dest.FirstName} {dest.LastName}".Trim();
        var ics     = BuildIcs(p);
        var hasScad = p.Scadenze.Any();
        var html    = $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto">
              <div style="background:{p.Colore};color:#fff;padding:20px 28px;border-radius:10px 10px 0 0">
                <h2 style="margin:0">📋 {p.Titolo}</h2>
                <p style="margin:6px 0 0;opacity:.85">{motivazione}</p>
              </div>
              <div style="background:#f8f9fc;padding:20px 28px;border-radius:0 0 10px 10px">
                <p>Ciao <strong>{nome}</strong>,</p>
                <p>{motivazione.TrimEnd('.')}.</p>
                {(string.IsNullOrEmpty(p.Descrizione) ? "" : $"<p><em>{p.Descrizione}</em></p>")}
                {(hasScad
                    ? $"<p>In allegato trovi il file <strong>.ics</strong> con le scadenze del progetto:<br/>puoi aprirlo per aggiungere tutti gli eventi al tuo <strong>Google Calendar</strong>, Outlook o Apple Calendar.</p><ul>{string.Concat(p.Scadenze.OrderBy(s => s.Data).Select(s => $"<li><strong>{s.Data:dd/MM/yyyy}</strong> — {s.Titolo}</li>"))}</ul>"
                    : "<p>Non ci sono ancora scadenze. Riceverai un aggiornamento non appena verranno aggiunte.</p>")}
                <p style="color:#888;font-size:.85rem;margin-top:20px">— Team LTB</p>
              </div>
            </div>
            """;
        var slug = p.Titolo.ToLowerInvariant().Replace(" ", "-");
        try { await emailSvc.SendWithIcsAsync(dest.Email, $"📋 Progetto: {p.Titolo}", html, ics, $"ltb-{slug}.ics"); }
        catch { /* email non bloccante */ }
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

        // Email al referente con ICS (progetto appena creato, nessuna scadenza ancora)
        if (p.ReferenteId != null)
        {
            var ref_ = await _userManager.FindByIdAsync(p.ReferenteId);
            if (ref_ != null)
                await InviaEmailCalendario(_email, p, ref_,
                    $"Sei stato indicato come referente del progetto \"{p.Titolo}\"");
        }

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

        // Ricarica progetto con scadenze aggiornate e invia ICS a tutti i membri
        var p = await _db.Progetti
            .Include(x => x.Scadenze)
            .Include(x => x.Membri)
            .FirstOrDefaultAsync(x => x.IdProgetto == idProgetto);
        if (p != null)
        {
            foreach (var m in p.Membri)
            {
                var u = await _userManager.FindByIdAsync(m.UserId);
                if (u != null)
                    await InviaEmailCalendario(_email, p, u,
                        $"Le scadenze del progetto \"{p.Titolo}\" sono state aggiornate");
            }
        }

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

            // Email al nuovo membro con ICS delle scadenze attuali
            var p = await _db.Progetti
                .Include(x => x.Scadenze)
                .FirstOrDefaultAsync(x => x.IdProgetto == idProgetto);
            var newUser = await _userManager.FindByIdAsync(userId);
            if (p != null && newUser != null)
                await InviaEmailCalendario(_email, p, newUser,
                    $"Sei stato aggiunto al progetto \"{p.Titolo}\"");
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

    // ── Export ICS ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> ExportIcs(int idProgetto)
    {
        var p = await _db.Progetti
            .Include(p => p.Scadenze)
            .FirstOrDefaultAsync(p => p.IdProgetto == idProgetto);
        if (p == null) return NotFound();

        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//LTB//Coordinazione//IT");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");

        foreach (var s in p.Scadenze.OrderBy(s => s.Data))
        {
            var dateStr  = s.Data.ToString("yyyyMMdd");
            var dateNext = s.Data.AddDays(1).ToString("yyyyMMdd");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:ltb-scad-{s.IdScadenza}@ltb");
            sb.AppendLine($"DTSTART;VALUE=DATE:{dateStr}");
            sb.AppendLine($"DTEND;VALUE=DATE:{dateNext}");
            sb.AppendLine($"SUMMARY:{EscapeIcs(s.Titolo)} [{EscapeIcs(p.Titolo)}]");
            if (!string.IsNullOrEmpty(s.Nota))
                sb.AppendLine($"DESCRIPTION:{EscapeIcs(s.Nota)}");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var slug  = p.Titolo.ToLowerInvariant().Replace(" ", "-");
        return File(bytes, "text/calendar", $"ltb-{slug}.ics");
    }

    private static string EscapeIcs(string s) =>
        s.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,").Replace("\n", "\\n");

    // ── Impostazioni Notifiche ────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Impostazioni()
    {
        var impostazioni = await _db.NotificheImpostazioni.ToListAsync();
        var tutti = await TuttiGliUtenti();

        var vm = new ImpostazioniNotificheVM
        {
            Impostazioni      = impostazioni.Select(i => new NotificaVM
            {
                IdNotifica    = i.IdNotifica,
                UserId        = i.UserId,
                NomeUtente    = tutti.FirstOrDefault(u => u.UserId == i.UserId)?.Nome ?? i.UserId,
                IntervalloOre = i.IntervalloOre,
                UltimoInvio   = i.UltimoInvio,
                Attiva        = i.Attiva
            }).ToList(),
            UtentiDisponibili = tutti
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AggiungiNotifica(string userId, int intervalloOre)
    {
        var exists = await _db.NotificheImpostazioni.AnyAsync(n => n.UserId == userId);
        if (!exists)
        {
            _db.NotificheImpostazioni.Add(new NotificaImpostazione
            {
                UserId        = userId,
                IntervalloOre = intervalloOre,
                Attiva        = true,
                DataCreazione = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Impostazioni");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleNotifica(int idNotifica)
    {
        var n = await _db.NotificheImpostazioni.FindAsync(idNotifica);
        if (n != null) { n.Attiva = !n.Attiva; await _db.SaveChangesAsync(); }
        return RedirectToAction("Impostazioni");
    }

    [HttpPost]
    public async Task<IActionResult> EliminaNotifica(int idNotifica)
    {
        var n = await _db.NotificheImpostazioni.FindAsync(idNotifica);
        if (n != null) { _db.NotificheImpostazioni.Remove(n); await _db.SaveChangesAsync(); }
        return RedirectToAction("Impostazioni");
    }

    [HttpPost]
    public async Task<IActionResult> InviaOra()
    {
        await _digest.RunDigest();
        TempData["Msg"] = "Digest inviato manualmente.";
        return RedirectToAction("Impostazioni");
    }
}
