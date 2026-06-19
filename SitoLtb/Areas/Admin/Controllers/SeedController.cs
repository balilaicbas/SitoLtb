using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Utilities;

namespace SitoLtb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = WebsiteRoles.WebsiteAdmin)]
public class SeedController : Controller
{
    private readonly LtbDbContext _ltb;
    public SeedController(LtbDbContext ltb) => _ltb = ltb;

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Run()
    {
        var log = new List<(string cls, string msg)>();
        void Ok(string msg)  => log.Add(("ok",  msg));
        void Err(string msg) => log.Add(("err", msg));

        // ── 1. Schemi e tabelle ───────────────────────────────────────────────
        foreach (var s in new[] { "anag", "hist", "orga" })
            try { await _ltb.Database.ExecuteSqlRawAsync($"IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name='{s}') EXEC('CREATE SCHEMA {s}')"); } catch { }

        await RunDdlList(log);

        // ── 2. Svuota in ordine inverso FK ───────────────────────────────────
        var deletes = new[]
        {
            "IF OBJECT_ID('orga.PartecipantiCorsi',  'U') IS NOT NULL DELETE FROM orga.PartecipantiCorsi",
            "IF OBJECT_ID('orga.PartecipantiTornei', 'U') IS NOT NULL DELETE FROM orga.PartecipantiTornei",
            "IF OBJECT_ID('hist.EloScores',           'U') IS NOT NULL DELETE FROM hist.EloScores",
            "IF OBJECT_ID('orga.Socio',               'U') IS NOT NULL DELETE FROM orga.Socio",
            "IF OBJECT_ID('orga.Corsi',               'U') IS NOT NULL DELETE FROM orga.Corsi",
            "IF OBJECT_ID('orga.Eventi',              'U') IS NOT NULL DELETE FROM orga.Eventi",
            "IF OBJECT_ID('anag.Fornitori',           'U') IS NOT NULL DELETE FROM anag.Fornitori",
            "IF OBJECT_ID('anag.Scuole',              'U') IS NOT NULL DELETE FROM anag.Scuole",
            "IF OBJECT_ID('anag.Giocatori',           'U') IS NOT NULL DELETE FROM anag.Giocatori",
            "IF OBJECT_ID('anag.Circoli',             'U') IS NOT NULL DELETE FROM anag.Circoli",
            "IF OBJECT_ID('anag.Livello',             'U') IS NOT NULL DELETE FROM anag.Livello",
            "IF OBJECT_ID('anag.TipoEvento',          'U') IS NOT NULL DELETE FROM anag.TipoEvento",
            "IF OBJECT_ID('anag.TipoScuola',          'U') IS NOT NULL DELETE FROM anag.TipoScuola",
        };
        foreach (var sql in deletes)
            try   { await _ltb.Database.ExecuteSqlRawAsync(sql); }
            catch (Exception ex) { Err($"Delete: {ex.Message[..Math.Min(80, ex.Message.Length)]}"); }
        Ok("Tabelle svuotate");

        // ── 3. Riferimenti ───────────────────────────────────────────────────
        _ltb.TipiEvento.AddRange(
            new LtbTipoEvento { Descrizione = "Torneo" },
            new LtbTipoEvento { Descrizione = "Evento sociale" },
            new LtbTipoEvento { Descrizione = "Simul" });
        await _ltb.SaveChangesAsync();
        int tipoTorneoId  = (await _ltb.TipiEvento.FirstAsync(t => t.Descrizione == "Torneo"         )).IdTipoEvento;
        int tipoSocialeId = (await _ltb.TipiEvento.FirstAsync(t => t.Descrizione == "Evento sociale" )).IdTipoEvento;
        int tipoSimulId   = (await _ltb.TipiEvento.FirstAsync(t => t.Descrizione == "Simul"          )).IdTipoEvento;

        _ltb.Livelli.AddRange(
            new LtbLivello { IdLivello = 1, Descrizione = "Principiante" },
            new LtbLivello { IdLivello = 2, Descrizione = "Intermedio" },
            new LtbLivello { IdLivello = 3, Descrizione = "Avanzato" },
            new LtbLivello { IdLivello = 4, Descrizione = "Agonistico" });
        await _ltb.SaveChangesAsync();

        _ltb.TipiScuola.Add(new LtbTipoScuola { Descrizione = "Interna" });
        await _ltb.SaveChangesAsync();
        int tipoScuolaId = (await _ltb.TipiScuola.FirstAsync()).IdTipoScuola;

        _ltb.Scuole.Add(new LtbScuola { Nome = "Scuola LTB", Descrizione = "Interna", IdTipoScuola = tipoScuolaId });
        await _ltb.SaveChangesAsync();
        int scuolaId = (await _ltb.Scuole.FirstAsync()).IdScuola;

        _ltb.Circoli.AddRange(
            new LtbCircolo { RagioneSociale = "LTB - Libero Torneificio del Borgo",     Localita = "Torino", Note = "LTB       " },
            new LtbCircolo { RagioneSociale = "FSI - Federazione Scacchistica Italiana", Localita = "Roma",   Note = "FSI       " });
        await _ltb.SaveChangesAsync();
        var circoli   = await _ltb.Circoli.ToListAsync();
        int ltbCircId = circoli.First(c => (c.Note ?? "").TrimEnd() == "LTB").IdCircolo;
        int fsiCircId = circoli.First(c => (c.Note ?? "").TrimEnd() == "FSI").IdCircolo;
        Ok("Dati di riferimento: OK");

        var rng = new Random(42);

        // ── 4. Giocatori × 1000 ──────────────────────────────────────────────
        _ltb.Giocatori.AddRange(Gen1000Giocatori(rng));
        await _ltb.SaveChangesAsync();
        var playerIds = await _ltb.Giocatori.Select(g => g.IdGiocatore).ToListAsync();
        Ok($"Giocatori: {playerIds.Count}");

        // ── 5. EloScores × 1000 ──────────────────────────────────────────────
        _ltb.EloScores.AddRange(Gen1000EloScores(playerIds, rng));
        await _ltb.SaveChangesAsync();
        Ok("EloScores: 1000");

        // ── 6. Soci × 1000 ───────────────────────────────────────────────────
        _ltb.Soci.AddRange(Gen1000Soci(playerIds, ltbCircId, fsiCircId));
        await _ltb.SaveChangesAsync();
        Ok("Soci: 1000");

        // ── 7. Eventi × 1000 (no identity) ───────────────────────────────────
        _ltb.Eventi.AddRange(Gen1000Eventi(tipoTorneoId, tipoSocialeId, tipoSimulId, rng));
        await _ltb.SaveChangesAsync();
        var eventIds = await _ltb.Eventi.Select(e => e.IdEvento).ToListAsync();
        Ok($"Eventi: {eventIds.Count}");

        // ── 8. PartecipantiTornei × 1000 ─────────────────────────────────────
        _ltb.PartecipantiTornei.AddRange(Gen1000PartTornei(playerIds, eventIds, rng));
        await _ltb.SaveChangesAsync();
        Ok("PartecipantiTornei: 1000");

        // ── 9. Corsi × 1000 ──────────────────────────────────────────────────
        var livelliIds = await _ltb.Livelli.Select(l => l.IdLivello).ToListAsync();
        _ltb.Corsi.AddRange(Gen1000Corsi(livelliIds, scuolaId, rng));
        await _ltb.SaveChangesAsync();
        var corsoIds = await _ltb.Corsi.Select(c => c.IdCorso).ToListAsync();
        Ok($"Corsi: {corsoIds.Count}");

        // ── 10. PartecipantiCorsi × 1000 ─────────────────────────────────────
        _ltb.PartecipantiCorsi.AddRange(Gen1000PartCorsi(playerIds, corsoIds, rng));
        await _ltb.SaveChangesAsync();
        Ok("PartecipantiCorsi: 1000");

        // ── 11. Fornitori × 1000 ─────────────────────────────────────────────
        _ltb.Fornitori.AddRange(Gen1000Fornitori(playerIds, rng));
        await _ltb.SaveChangesAsync();
        Ok("Fornitori: 1000");

        ViewBag.Log = log;
        return View("Result");
    }

    // ═══════════════════════════════════════════ DDL ══════════════════════════

    private async Task RunDdlList(List<(string, string)> log)
    {
        var tables = new (string schema, string name, string ddl)[]
        {
            ("anag","TipoEvento",       "CREATE TABLE anag.TipoEvento (IdTipoEvento INT IDENTITY(1,1) CONSTRAINT PK_TipoEvento PRIMARY KEY, Descrizione NVARCHAR(100) NULL)"),
            ("anag","Livello",          "CREATE TABLE anag.Livello (IdLivello INT NOT NULL CONSTRAINT PK_Livello PRIMARY KEY, Descrizione NVARCHAR(150) NOT NULL)"),
            ("anag","TipoScuola",       "CREATE TABLE anag.TipoScuola (IdTipoScuola INT IDENTITY(1,1) CONSTRAINT PK_TipoScuola PRIMARY KEY, Descrizione NVARCHAR(100) NULL)"),
            ("anag","Circoli",          "CREATE TABLE anag.Circoli (IdCircolo INT IDENTITY(1,1) CONSTRAINT PK_Circoli PRIMARY KEY, RagioneSociale NVARCHAR(100) NOT NULL, Indirizzo NVARCHAR(100) NULL, [Località] NVARCHAR(100) NULL, Note CHAR(10) NULL)"),
            ("anag","Giocatori",        "CREATE TABLE anag.Giocatori (IdGiocatore INT IDENTITY(1,1) CONSTRAINT PK_Giocatori PRIMARY KEY, IdFide INT NULL, IdFSI INT NULL, IdNazionalita INT NULL, Nome NVARCHAR(50) NULL, Cognome NVARCHAR(50) NULL, Indirizzo NVARCHAR(100) NULL, DataNascita DATE NULL, Email NVARCHAR(50) NULL, NumeroCivico CHAR(10) NULL, CAP INT NULL, Localita NVARCHAR(100) NULL)"),
            ("anag","Scuole",           "CREATE TABLE anag.Scuole (IdScuola INT IDENTITY(1,1) CONSTRAINT PK_Scuole PRIMARY KEY, Nome NVARCHAR(50) NOT NULL, Descrizione NVARCHAR(50) NULL, IdTipoScuola INT NULL)"),
            ("anag","Fornitori",        "CREATE TABLE anag.Fornitori (IdFornitore INT IDENTITY(1,1) CONSTRAINT PK_Fornitori PRIMARY KEY, IdGiocatore INT NULL, RagioneSociale NVARCHAR(100) NOT NULL, Indirizzo NVARCHAR(100) NULL, NumeroCivico CHAR(10) NULL, Email NVARCHAR(50) NULL)"),
            ("hist","EloScores",        "CREATE TABLE hist.EloScores (IdScore INT IDENTITY(1,1) CONSTRAINT PK_EloScores PRIMARY KEY, IdGiocatore INT NOT NULL, DataAcquisizione DATE NOT NULL, EloStandard INT NULL, EloRapid INT NULL, EloBlitz INT NULL)"),
            ("orga","Socio",            "CREATE TABLE orga.Socio (IdSocio INT IDENTITY(1,1) CONSTRAINT PK_Socio PRIMARY KEY, IdGiocatore INT NOT NULL, IdCircolo INT NOT NULL, Anno INT NOT NULL)"),
            ("orga","Eventi",           "CREATE TABLE orga.Eventi (IdEvento INT NOT NULL CONSTRAINT PK_Eventi PRIMARY KEY, Descrizione NVARCHAR(500) NOT NULL, IdTipoEvento INT NOT NULL, DataInizio DATE NOT NULL, DataFine DATE NOT NULL)"),
            ("orga","PartecipantiTornei","CREATE TABLE orga.PartecipantiTornei (IdPartecipanteTornei INT IDENTITY(1,1) CONSTRAINT PK_PartecipantiTornei PRIMARY KEY, IdGiocatore INT NOT NULL, IdEvento INT NOT NULL)"),
            ("orga","Corsi",            "CREATE TABLE orga.Corsi (IdCorso INT IDENTITY(1,1) CONSTRAINT PK_Corsi PRIMARY KEY, IdLivello INT NOT NULL, Descrizione NVARCHAR(500) NOT NULL, DataInizio DATE NOT NULL, Datafine DATE NOT NULL, IdScuola INT NOT NULL)"),
            ("orga","PartecipantiCorsi","CREATE TABLE orga.PartecipantiCorsi (IdCorso INT NOT NULL, IdGiocatore INT NOT NULL, CONSTRAINT PK_PartecipantiCorsi PRIMARY KEY (IdCorso, IdGiocatore))"),
        };
        foreach (var (schema, name, ddl) in tables)
        {
            try
            {
                await _ltb.Database.ExecuteSqlRawAsync($@"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id=s.schema_id
                                   WHERE s.name='{schema}' AND t.name='{name}')
                    {ddl}");
            }
            catch (Exception ex) { log.Add(("err", $"{schema}.{name}: {ex.Message[..Math.Min(60, ex.Message.Length)]}")); }
        }
    }

    // ═══════════════════════════════════════════ GENERATORS ═══════════════════

    private static readonly string[] Nomi = [
        "Marco","Luca","Andrea","Giovanni","Francesco","Antonio","Matteo","Davide",
        "Lorenzo","Simone","Riccardo","Emanuele","Nicola","Stefano","Cristian",
        "Massimo","Roberto","Claudio","Giorgio","Filippo","Alessandro","Daniele",
        "Fabio","Giuseppe","Luigi","Mario","Carlo","Paolo","Sergio","Federico",
        "Giacomo","Leonardo","Mattia","Tommaso","Enrico","Vittorio","Dario","Elia",
        "Flavio","Ivan"
    ];

    private static readonly string[] Cognomi = [
        "Rossi","Ferrari","Bianchi","Russo","Colombo","Ricci","Marino","Greco",
        "Conti","Esposito","Mancini","Costa","Giordano","Rizzo","Ferrara",
        "De Luca","Bruno","Moretti","Leone","Serra","Gentile","Fontana",
        "Caruso","Pellegrini","Ferretti"
    ];

    private static readonly string[] Citta = [
        "Torino","Milano","Roma","Napoli","Bologna","Firenze","Venezia","Genova",
        "Palermo","Bari","Catania","Verona","Padova","Trieste","Brescia","Parma"
    ];

    private static IEnumerable<LtbGiocatore> Gen1000Giocatori(Random rng)
    {
        // 40 nomi × 25 cognomi = 1000
        for (int i = 0; i < 1000; i++)
        {
            string nome    = Nomi[i % 40];
            string cognome = Cognomi[i / 40];
            yield return new LtbGiocatore
            {
                Nome        = nome,
                Cognome     = cognome,
                DataNascita = new DateOnly(rng.Next(1950, 2010), rng.Next(1, 13), rng.Next(1, 28)),
                Email       = i < 600 ? $"{nome.ToLower()}.{cognome.ToLower().Replace(" ", "")}{i}@ltb.it" : null,
                Localita    = Citta[i % Citta.Length],
                IdFSI       = i < 400 ? 10000 + i : null,
                IdFide      = i < 200 ? 50000 + i : null,
            };
        }
    }

    private static readonly DateOnly[] EloDates =
        [new(2023,3,1), new(2023,10,1), new(2024,4,1), new(2024,10,1), new(2025,4,1)];

    private static IEnumerable<LtbEloScore> Gen1000EloScores(List<int> playerIds, Random rng)
    {
        // 1 snapshot per giocatore = 1000 righe
        // distribuzione realistica: media 1200, code in 800-2100
        var weights = new[] { 5, 10, 20, 30, 20, 10, 5 };      // fascia <1000 … 2000+
        var bases   = new[] { 900, 1100, 1300, 1500, 1700, 1900, 2100 };
        int totalW  = weights.Sum();

        foreach (var pid in playerIds)
        {
            int bucket = WeightedPick(rng, weights, totalW);
            int std    = bases[bucket] + rng.Next(-100, 100);
            yield return new LtbEloScore
            {
                IdGiocatore      = pid,
                DataAcquisizione = EloDates[rng.Next(EloDates.Length)],
                EloStandard      = std,
                EloRapid         = std + rng.Next(-80,  50),
                EloBlitz         = std + rng.Next(-120, 20),
            };
        }
    }

    private static IEnumerable<LtbSocio> Gen1000Soci(List<int> playerIds, int ltbId, int fsiId)
    {
        // LTB per anno: 150 + 200 + 250 + 200 = 800
        // FSI per anno: 50 + 50 + 100          = 200
        // Totale: 1000
        var ltbByYear = new Dictionary<int, (int start, int count)>
        {
            [2022] = (0,   150),
            [2023] = (0,   200),
            [2024] = (0,   250),
            [2025] = (50,  200),
        };
        var fsiByYear = new Dictionary<int, (int start, int count)>
        {
            [2023] = (20,  50),
            [2024] = (20,  50),
            [2025] = (20, 100),
        };

        foreach (var (anno, (start, count)) in ltbByYear)
            for (int i = start; i < start + count && i < playerIds.Count; i++)
                yield return new LtbSocio { IdGiocatore = playerIds[i], IdCircolo = ltbId, Anno = anno };

        foreach (var (anno, (start, count)) in fsiByYear)
            for (int i = start; i < start + count && i < playerIds.Count; i++)
                yield return new LtbSocio { IdGiocatore = playerIds[i], IdCircolo = fsiId, Anno = anno };
    }

    private static readonly string[] TorneoAdj  = ["Standard","Rapid","Blitz","Eterodosso","Open","Lampo","Semilampo","Classico"];
    private static readonly string[] TorneoNoun  = ["Inverno","Primavera","Estate","Autunno","Apertura","Chiusura","Natalizio","Pasquale","Memorial","Gran Prix","Campionato","Challenge"];

    private static List<LtbEvento> Gen1000Eventi(int tipoTorneo, int tipoSociale, int tipoSimul, Random rng)
    {
        var list  = new List<LtbEvento>();
        int id    = 1;
        // 250 eventi per anno, 2022-2025
        for (int year = 2022; year <= 2025; year++)
        {
            for (int n = 0; n < 250; n++)
            {
                int  month   = n % 12 + 1;
                int  day     = rng.Next(1, 26);
                var  data    = new DateOnly(year, month, day);
                int  tipo;
                string desc;

                int roll = rng.Next(100);
                if (roll < 70)
                {
                    tipo = tipoTorneo;
                    desc = $"Torneo {TorneoAdj[rng.Next(TorneoAdj.Length)]} {TorneoNoun[rng.Next(TorneoNoun.Length)]} {year}";
                }
                else if (roll < 85)
                {
                    tipo = tipoSimul;
                    desc = $"Simul con {(rng.Next(2) == 0 ? "MF" : "FM")} {Cognomi[rng.Next(Cognomi.Length)]} {year}";
                }
                else
                {
                    tipo = tipoSociale;
                    desc = $"Evento sociale LTB {year} n.{n}";
                }

                list.Add(new LtbEvento
                {
                    IdEvento     = id++,
                    Descrizione  = desc,
                    IdTipoEvento = tipo,
                    DataInizio   = data,
                    DataFine     = data,
                });
            }
        }
        return list;
    }

    private static List<LtbPartecipanteTorneo> Gen1000PartTornei(List<int> playerIds, List<int> eventIds, Random rng)
    {
        var used = new HashSet<(int, int)>();
        var list = new List<LtbPartecipanteTorneo>();
        int attempts = 0;
        while (list.Count < 1000 && attempts < 50_000)
        {
            int ev = eventIds[rng.Next(eventIds.Count)];
            int pl = playerIds[rng.Next(playerIds.Count)];
            if (used.Add((ev, pl)))
                list.Add(new LtbPartecipanteTorneo { IdEvento = ev, IdGiocatore = pl });
            attempts++;
        }
        return list;
    }

    private static readonly string[] CorsoNames = ["Principianti","Intermedio","Avanzato","Agonistico","Aperture","Finali","Tattiche","Endgame","Scuola Estiva","Corso Intensivo"];

    private static IEnumerable<LtbCorso> Gen1000Corsi(List<int> livelliIds, int scuolaId, Random rng)
    {
        for (int i = 0; i < 1000; i++)
        {
            int year   = 2022 + (i / 250);
            int month  = (i % 12) + 1;
            var inizio = new DateOnly(year, month, 1);
            var fine   = inizio.AddMonths(3);
            yield return new LtbCorso
            {
                IdLivello   = livelliIds[rng.Next(livelliIds.Count)],
                Descrizione = $"Corso {CorsoNames[i % CorsoNames.Length]} {year}/{i % 250 + 1}",
                DataInizio  = inizio,
                DataFine    = fine,
                IdScuola    = scuolaId,
            };
        }
    }

    private static List<LtbPartecipanteCorso> Gen1000PartCorsi(List<int> playerIds, List<int> corsoIds, Random rng)
    {
        var used = new HashSet<(int, int)>();
        var list = new List<LtbPartecipanteCorso>();
        int attempts = 0;
        while (list.Count < 1000 && attempts < 50_000)
        {
            int corso  = corsoIds[rng.Next(corsoIds.Count)];
            int player = playerIds[rng.Next(playerIds.Count)];
            if (used.Add((corso, player)))
                list.Add(new LtbPartecipanteCorso { IdCorso = corso, IdGiocatore = player });
            attempts++;
        }
        return list;
    }

    private static readonly string[] FornTipi  = ["Tipografia","Studio Grafico","Web Agency","Stamperia","Agenzia","Officina","Laboratorio","Centro","Consulenza","Distribuzione"];
    private static readonly string[] FornNomi  = ["Nord","Sud","Est","Ovest","Centrale","Moderna","Digitale","Classica","Rapida","Elite","Pro","Plus"];

    private static IEnumerable<LtbFornitore> Gen1000Fornitori(List<int> playerIds, Random rng)
    {
        for (int i = 0; i < 1000; i++)
        {
            bool isGiocatore = i < 50; // prime 50 sono anche giocatori
            yield return new LtbFornitore
            {
                IdGiocatore    = isGiocatore ? playerIds[i % playerIds.Count] : null,
                RagioneSociale = $"{FornTipi[i % FornTipi.Length]} {FornNomi[(i / FornTipi.Length) % FornNomi.Length]} {i + 1}",
                Email          = $"info{i + 1}@fornitore.it",
                Indirizzo      = $"Via {Cognomi[i % Cognomi.Length]} {rng.Next(1, 200)}",
            };
        }
    }

    private static int WeightedPick(Random rng, int[] weights, int total)
    {
        int r = rng.Next(total);
        int cum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            cum += weights[i];
            if (r < cum) return i;
        }
        return weights.Length - 1;
    }
}
