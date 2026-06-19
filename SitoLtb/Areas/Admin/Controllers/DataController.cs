using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Utilities;
using SitoLtb.ViewModels;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRoles.WebsiteAdmin + "," + WebsiteRoles.Data)]
    public class DataController : Controller
    {
        private readonly LtbDbContext _ltb;

        public DataController(LtbDbContext ltb) => _ltb = ltb;

        public async Task<IActionResult> Index()
        {
            // ── Carico in sequenza — tabelle opzionali fallback a lista vuota ──
            static async Task<List<T>> Try<T>(Func<Task<List<T>>> query) where T : class
            {
                try   { return await query(); }
                catch { return []; }
            }

            var eventi     = await Try(() => _ltb.Eventi.Include(e => e.TipoEvento).ToListAsync());
            var partTornei = await Try(() => _ltb.PartecipantiTornei.ToListAsync());
            var soci       = await Try(() => _ltb.Soci.Include(s => s.Circolo).ToListAsync());
            var giocatori  = await Try(() => _ltb.Giocatori.Include(g => g.EloScores).ToListAsync());
            var corsi      = await Try(() => _ltb.Corsi.Include(c => c.Livello).Include(c => c.Partecipanti).ToListAsync());
            var fornitori  = await Try(() => _ltb.Fornitori.ToListAsync());

            // ── Anno da mostrare: corrente se ha dati, altrimenti il più recente con tornei ──
            int year = DateTime.Today.Year;
            if (eventi.Any() && !eventi.Any(e => e.DataInizio.Year == year))
                year = eventi.Max(e => e.DataInizio.Year);

            var from = new DateOnly(year, 1, 1);
            var to   = new DateOnly(year, 12, 31);

            // ── TORNEI ───────────────────────────────────────────────────────
            var torneiAnno = eventi
                .Where(e => e.DataInizio >= from && e.DataInizio <= to && e.IdTipoEvento == 1)
                .OrderBy(e => e.DataInizio)
                .ToList();

            var partPerEvento = partTornei
                .GroupBy(p => p.IdEvento)
                .ToDictionary(g => g.Key, g => g.Count());

            // Limita il bar chart ai 20 tornei con più partecipanti (evita centinaia di barre)
            var torneiChart = torneiAnno
                .OrderByDescending(e => partPerEvento.GetValueOrDefault(e.IdEvento, 0))
                .Take(20)
                .ToList();

            var mesiNomi = new[] { "Gen","Feb","Mar","Apr","Mag","Giu","Lug","Ago","Set","Ott","Nov","Dic" };
            var torneiPerMese = Enumerable.Range(1, 12)
                .Select(m => torneiAnno.Count(e => e.DataInizio.Month == m))
                .ToArray();
            var partecipazioniPerMese = Enumerable.Range(1, 12)
                .Select(m => torneiAnno
                    .Where(e => e.DataInizio.Month == m)
                    .Sum(e => partPerEvento.GetValueOrDefault(e.IdEvento, 0)))
                .ToArray();

            var tipiEvento = eventi
                .Where(e => e.DataInizio >= from && e.DataInizio <= to)
                .GroupBy(e => e.TipoEvento?.Descrizione ?? "Altro")
                .Select(g => (Tipo: g.Key, Count: g.Count()))
                .OrderByDescending(g => g.Count)
                .ToList();

            // ── SOCI ─────────────────────────────────────────────────────────
            var allCircoli = soci
                .Select(s => s.Circolo)
                .OfType<LtbCircolo>()
                .DistinctBy(c => c.IdCircolo)
                .ToList();

            static bool HasKeyword(string text, string kw) =>
                text.Contains(kw, StringComparison.OrdinalIgnoreCase);

            var ltbIds = allCircoli
                .Where(c => HasKeyword(c.RagioneSociale, "LTB")
                         || HasKeyword(c.RagioneSociale, "Torneificio")
                         || HasKeyword(c.Note?.Trim() ?? "", "LTB"))
                .Select(c => c.IdCircolo).ToHashSet();

            var fsiIds = allCircoli
                .Where(c => HasKeyword(c.RagioneSociale, "FSI")
                         || HasKeyword(c.RagioneSociale, "Federazione")
                         || HasKeyword(c.Note?.Trim() ?? "", "FSI"))
                .Select(c => c.IdCircolo).ToHashSet();

            var sociAnno  = soci.Where(s => s.Anno == year).ToList();
            var ltbPlayers = sociAnno.Where(s => ltbIds.Contains(s.IdCircolo)).Select(s => s.IdGiocatore).ToHashSet();
            var fsiPlayers = sociAnno.Where(s => fsiIds.Contains(s.IdCircolo)).Select(s => s.IdGiocatore).ToHashSet();

            // Trend tessere (LTB se trovato, altrimenti tutti i circoli)
            var filterIds = ltbIds.Count > 0 ? ltbIds : (HashSet<int>?)null;
            var sociTrend = soci
                .Where(s => filterIds == null || filterIds.Contains(s.IdCircolo))
                .GroupBy(s => s.Anno)
                .OrderBy(g => g.Key)
                .Select(g => (Anno: g.Key.ToString(), Count: g.Count()))
                .ToList();

            // ── GIOCATORI & ELO ──────────────────────────────────────────────
            var latestElo = giocatori.Select(g => (
                Giocatore: g,
                Elo: g.EloScores.OrderByDescending(e => e.DataAcquisizione).FirstOrDefault()
            )).ToList();

            var eloRanges = new[] { "<1000", "1000–1200", "1200–1400", "1400–1600", "1600–1800", "1800–2000", "2000+" };
            var eloCounts = new int[7];
            foreach (var (_, elo) in latestElo.Where(x => x.Elo?.EloStandard != null))
            {
                int v = elo!.EloStandard!.Value;
                int i = v < 1000 ? 0 : v < 1200 ? 1 : v < 1400 ? 2 : v < 1600 ? 3 : v < 1800 ? 4 : v < 2000 ? 5 : 6;
                eloCounts[i]++;
            }

            var topElo = latestElo
                .Where(x => x.Elo?.EloStandard != null)
                .OrderByDescending(x => x.Elo!.EloStandard)
                .Take(10)
                .Select(x => (
                    Nome: $"{x.Giocatore.Nome} {x.Giocatore.Cognome}".Trim(),
                    Elo: x.Elo!.EloStandard!.Value
                ))
                .ToList();

            var giocatoriById = giocatori.ToDictionary(g => g.IdGiocatore);
            var topPart = partTornei
                .GroupBy(p => p.IdGiocatore)
                .Select(g => (Id: g.Key, Count: g.Count()))
                .OrderByDescending(g => g.Count)
                .Take(10)
                .Select(g => (
                    Nome: giocatoriById.TryGetValue(g.Id, out var gio)
                        ? $"{gio.Nome} {gio.Cognome}".Trim()
                        : $"#{g.Id}",
                    Count: g.Count
                ))
                .ToList();

            // ── CORSI ────────────────────────────────────────────────────────
            var corsiAnno = corsi.Where(c => c.DataInizio >= from && c.DataInizio <= to).ToList();
            // Top 20 per il bar chart (evita centinaia di barre)
            var corsiChart = corsiAnno
                .OrderByDescending(c => c.Partecipanti.Count)
                .Take(20)
                .ToList();
            var corsiPerLivello = corsi
                .GroupBy(c => c.Livello?.Descrizione ?? "N/D")
                .Select(g => (Livello: g.Key, Count: g.Count()))
                .OrderByDescending(g => g.Count)
                .ToList();

            // ── VM ───────────────────────────────────────────────────────────
            var giocatoriIds = giocatori.Select(g => g.IdGiocatore).ToHashSet();

            var vm = new DataDashboardVM
            {
                Anno = year,

                TotaleGiocatori      = giocatori.Count,
                SociLtb              = ltbPlayers.Count,
                SociFsi              = fsiPlayers.Count,
                TotaleTornei         = torneiAnno.Count,
                TotalePartecipazioni = torneiAnno.Sum(e => partPerEvento.GetValueOrDefault(e.IdEvento, 0)),
                TotaleCorsi          = corsiAnno.Count,
                TotaleFornitori      = fornitori.Count,

                TorneiLabels = torneiChart.Select(e => e.Descrizione).ToArray(),
                TorneiCounts = torneiChart.Select(e => partPerEvento.GetValueOrDefault(e.IdEvento, 0)).ToArray(),

                MesiLabels            = mesiNomi,
                TorneiPerMese         = torneiPerMese,
                PartecipazioniPerMese = partecipazioniPerMese,

                TipiEventoLabels = tipiEvento.Select(t => t.Tipo).ToArray(),
                TipiEventoCounts = tipiEvento.Select(t => t.Count).ToArray(),

                SociAnniLabels = sociTrend.Select(s => s.Anno).ToArray(),
                SociPerAnno    = sociTrend.Select(s => s.Count).ToArray(),

                SoloLtb        = ltbPlayers.Except(fsiPlayers).Count(),
                SoloFsi        = fsiPlayers.Except(ltbPlayers).Count(),
                EntrambiLtbFsi = ltbPlayers.Intersect(fsiPlayers).Count(),

                EloRangeLabels   = eloRanges,
                EloDistribuzione = eloCounts,

                TopEloNomi   = topElo.Select(e => e.Nome).ToArray(),
                TopEloValori = topElo.Select(e => e.Elo).ToArray(),

                TopPartNomi   = topPart.Select(p => p.Nome).ToArray(),
                TopPartValori = topPart.Select(p => p.Count).ToArray(),

                CorsiLabels       = corsiChart.Select(c => c.Descrizione).ToArray(),
                CorsiPartecipanti = corsiChart.Select(c => c.Partecipanti.Count).ToArray(),

                LivelliLabels   = corsiPerLivello.Select(c => c.Livello).ToArray(),
                CorsiPerLivello = corsiPerLivello.Select(c => c.Count).ToArray(),

                Fornitori = fornitori
                    .OrderBy(f => f.RagioneSociale)
                    .Select(f => new DataDashboardVM.FornitoreRow(
                        f.RagioneSociale,
                        f.Email,
                        f.IdGiocatore.HasValue && giocatoriIds.Contains(f.IdGiocatore.Value)
                    )).ToList()
            };

            ViewData["Title"] = $"Dashboard — {year}";
            return View(vm);
        }
    }
}
