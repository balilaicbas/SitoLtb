using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using System.Text;

namespace SitoLtb.Services;

public class CoordinazioneDigestService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CoordinazioneDigestService> _logger;

    public CoordinazioneDigestService(IServiceScopeFactory scopeFactory,
        ILogger<CoordinazioneDigestService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Controlla ogni ora se ci sono digest da inviare
        while (!ct.IsCancellationRequested)
        {
            try { await RunDigest(ct); }
            catch (Exception ex) { _logger.LogError(ex, "Errore nel digest coordinazione"); }

            await Task.Delay(TimeSpan.FromHours(1), ct);
        }
    }

    public async Task RunDigest(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var email       = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var now = DateTime.UtcNow;

        // Impostazioni globali configurate (digest personalizzato per utente)
        var impostazioni = await db.NotificheImpostazioni
            .Where(n => n.Attiva && (n.UltimoInvio == null ||
                n.UltimoInvio.Value.AddHours(n.IntervalloOre) <= now))
            .ToListAsync(ct);

        if (!impostazioni.Any()) return;

        var progetti = await db.Progetti
            .Include(p => p.Tasks)
            .Include(p => p.Scadenze)
            .Include(p => p.Note)
            .Include(p => p.Membri)
            .OrderByDescending(p => p.DataCreazione)
            .ToListAsync(ct);

        foreach (var imp in impostazioni)
        {
            var user = await userManager.FindByIdAsync(imp.UserId);
            if (user?.Email == null) continue;

            var nome = $"{user.FirstName} {user.LastName}".Trim();
            var html = BuildDigestHtml(progetti, nome, now);

            try
            {
                await email.SendAsync(user.Email, $"📋 Digest Coordinazione LTB — {now:dd/MM/yyyy}", html);
                imp.UltimoInvio = now;
                _logger.LogInformation("Digest inviato a {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore invio digest a {Email}", user.Email);
            }
        }

        // Digest settimanale automatico ai membri dei singoli progetti
        var unWeekAgo = now.AddDays(-7);
        var progettiConMembri = progetti.Where(p => p.Membri.Any()).ToList();

        foreach (var p in progettiConMembri)
        {
            foreach (var m in p.Membri)
            {
                var memberUser = await userManager.FindByIdAsync(m.UserId);
                if (memberUser?.Email == null) continue;

                // Controlla se è già stato inviato questa settimana (via flag in UltimoInvio)
                // Usiamo le impostazioni personali per evitare duplicati se l'utente ha già una config globale
                if (impostazioni.Any(i => i.UserId == m.UserId)) continue;

                var nome = $"{memberUser.FirstName} {memberUser.LastName}".Trim();
                var html = BuildProgettoHtml(p, nome, now);

                try
                {
                    await email.SendAsync(memberUser.Email,
                        $"📋 Aggiornamento settimanale — {p.Titolo}", html);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore invio digest progetto {P} a {Email}", p.Titolo, memberUser.Email);
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }

    // ── HTML builders ─────────────────────────────────────────────────────────

    private static string BuildDigestHtml(List<Progetto> progetti, string nomeUtente, DateTime now)
    {
        var oggi = DateOnly.FromDateTime(now);
        var sb = new StringBuilder();
        sb.Append($"""
            <div style="font-family:sans-serif;max-width:680px;margin:auto">
            <div style="background:#4e73df;color:#fff;padding:24px 32px;border-radius:12px 12px 0 0">
                <h1 style="margin:0;font-size:22px">📋 Digest Coordinazione LTB</h1>
                <p style="margin:6px 0 0;opacity:.85">{now:dddd dd MMMM yyyy} — Ciao {nomeUtente}!</p>
            </div>
            <div style="background:#f8f9fc;padding:24px 32px">
            """);

        // Scadenze imminenti (prossimi 14 giorni)
        var scadenzeImminenti = progetti
            .SelectMany(p => p.Scadenze.Select(s => (Progetto: p, Scadenza: s)))
            .Where(x => x.Scadenza.Data >= oggi && x.Scadenza.Data <= oggi.AddDays(14))
            .OrderBy(x => x.Scadenza.Data)
            .ToList();

        if (scadenzeImminenti.Any())
        {
            sb.Append("""
                <h2 style="color:#e74a3b;margin-top:0">⏰ Scadenze imminenti (14 giorni)</h2>
                <table style="width:100%;border-collapse:collapse">
                """);
            foreach (var (prog, sc) in scadenzeImminenti)
            {
                var isUrgente = sc.Data <= oggi.AddDays(3);
                var color = isUrgente ? "#e74a3b" : "#f6c23e";
                sb.Append($"""
                    <tr>
                        <td style="padding:8px 0;border-bottom:1px solid #e9ecef">
                            <span style="background:{color};color:#fff;border-radius:4px;padding:2px 8px;font-size:12px">
                                {sc.Data:dd/MM}
                            </span>
                        </td>
                        <td style="padding:8px;border-bottom:1px solid #e9ecef;font-weight:600">{sc.Titolo}</td>
                        <td style="padding:8px;border-bottom:1px solid #e9ecef;color:#888">{prog.Titolo}</td>
                    </tr>
                    """);
            }
            sb.Append("</table><br/>");
        }

        // Riepilogo progetti
        sb.Append("<h2 style=\"color:#2d3a4a\">📁 Riepilogo progetti</h2>");
        foreach (var p in progetti.Where(p => p.Stato == StatoProgetto.Attivo))
            sb.Append(BuildProgettoCard(p, oggi));

        sb.Append("</div></div>");
        return sb.ToString();
    }

    private static string BuildProgettoHtml(Progetto p, string nomeUtente, DateTime now)
    {
        var oggi = DateOnly.FromDateTime(now);
        var sb = new StringBuilder();
        sb.Append($"""
            <div style="font-family:sans-serif;max-width:680px;margin:auto">
            <div style="background:{p.Colore};color:#fff;padding:24px 32px;border-radius:12px 12px 0 0">
                <h1 style="margin:0;font-size:20px">📋 {p.Titolo}</h1>
                <p style="margin:6px 0 0;opacity:.85">Aggiornamento settimanale — Ciao {nomeUtente}!</p>
            </div>
            <div style="background:#f8f9fc;padding:24px 32px">
            """);
        sb.Append(BuildProgettoCard(p, oggi));
        sb.Append("</div></div>");
        return sb.ToString();
    }

    private static string BuildProgettoCard(Progetto p, DateOnly oggi)
    {
        var totaleTask  = p.Tasks.Count;
        var taskRisolte = p.Tasks.Count(t => t.Stato == StatoTask.Risolto);
        var avanz       = totaleTask == 0 ? 0 : (int)Math.Round(taskRisolte * 100.0 / totaleTask);
        var statoColor  = p.Stato == StatoProgetto.Completato ? "#1cc88a"
                        : p.Stato == StatoProgetto.Sospeso    ? "#f6c23e" : "#4e73df";

        var scadImm = p.Scadenze
            .Where(s => s.Data >= oggi && s.Data <= oggi.AddDays(7))
            .OrderBy(s => s.Data).ToList();

        var taskInCorso = p.Tasks
            .Where(t => t.Stato == StatoTask.InCorso)
            .OrderBy(t => t.DataCreazione).Take(5).ToList();

        var sb = new StringBuilder();
        sb.Append($"""
            <div style="background:#fff;border-radius:10px;padding:18px 22px;margin-bottom:16px;
                        border-left:4px solid {p.Colore};box-shadow:0 2px 8px rgba(0,0,0,.07)">
                <div style="display:flex;justify-content:space-between;align-items:center">
                    <strong style="font-size:16px;color:#2d3a4a">{p.Titolo}</strong>
                    <span style="background:{statoColor};color:#fff;border-radius:20px;
                                 padding:2px 10px;font-size:12px">{p.Stato}</span>
                </div>
            """);

        if (!string.IsNullOrEmpty(p.Descrizione))
            sb.Append($"<p style=\"color:#666;margin:6px 0;font-size:14px\">{p.Descrizione}</p>");

        // Avanzamento
        sb.Append($"""
            <div style="margin:10px 0">
                <div style="display:flex;justify-content:space-between;font-size:12px;color:#888;margin-bottom:4px">
                    <span>Avanzamento</span><span>{avanz}%</span>
                </div>
                <div style="background:#e9ecef;border-radius:10px;height:8px">
                    <div style="background:{p.Colore};width:{avanz}%;height:8px;border-radius:10px"></div>
                </div>
            </div>
            """);

        // Task in corso
        if (taskInCorso.Any())
        {
            sb.Append("<p style=\"font-size:13px;font-weight:700;color:#555;margin:10px 0 4px\">Task in corso:</p><ul style=\"margin:0;padding-left:18px\">");
            foreach (var t in taskInCorso)
                sb.Append($"<li style=\"font-size:13px;color:#444;margin:2px 0\">{t.Titolo}</li>");
            sb.Append("</ul>");
        }

        // Scadenze imminenti
        if (scadImm.Any())
        {
            sb.Append("<p style=\"font-size:13px;font-weight:700;color:#e74a3b;margin:10px 0 4px\">⏰ Scadenze questa settimana:</p><ul style=\"margin:0;padding-left:18px\">");
            foreach (var s in scadImm)
                sb.Append($"<li style=\"font-size:13px;color:#444\">{s.Data:dd/MM} — {s.Titolo}</li>");
            sb.Append("</ul>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }
}
