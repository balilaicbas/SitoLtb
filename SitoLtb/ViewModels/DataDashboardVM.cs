namespace SitoLtb.ViewModels;

public class DataDashboardVM
{
    public int Anno { get; set; }

    // ── KPI ──────────────────────────────────────────────────────
    public int TotaleGiocatori      { get; set; }
    public int SociLtb              { get; set; }
    public int SociFsi              { get; set; }
    public int TotaleTornei         { get; set; }
    public int TotalePartecipazioni { get; set; }
    public int TotaleCorsi          { get; set; }
    public int TotaleFornitori      { get; set; }

    // ── Tornei — partecipanti per torneo ─────────────────────────
    public string[] TorneiLabels { get; set; } = [];
    public int[]    TorneiCounts { get; set; } = [];

    // ── Tornei — per mese ────────────────────────────────────────
    public string[] MesiLabels    { get; set; } = [];
    public int[]    TorneiPerMese { get; set; } = [];

    // ── Tipi evento — doughnut ───────────────────────────────────
    public string[] TipiEventoLabels { get; set; } = [];
    public int[]    TipiEventoCounts { get; set; } = [];

    // ── Soci — trend annuale ─────────────────────────────────────
    public string[] SociAnniLabels { get; set; } = [];
    public int[]    SociPerAnno    { get; set; } = [];

    // ── Soci — LTB / FSI / Entrambi ─────────────────────────────
    public int SoloLtb        { get; set; }
    public int SoloFsi        { get; set; }
    public int EntrambiLtbFsi { get; set; }

    // ── ELO — distribuzione Standard ─────────────────────────────
    public string[] EloRangeLabels   { get; set; } = [];
    public int[]    EloDistribuzione { get; set; } = [];

    // ── ELO — top 10 giocatori ───────────────────────────────────
    public string[] TopEloNomi   { get; set; } = [];
    public int[]    TopEloValori { get; set; } = [];

    // ── Partecipazioni — top 10 ──────────────────────────────────
    public string[] TopPartNomi   { get; set; } = [];
    public int[]    TopPartValori { get; set; } = [];

    // ── Corsi — partecipanti per corso ───────────────────────────
    public string[] CorsiLabels       { get; set; } = [];
    public int[]    CorsiPartecipanti { get; set; } = [];

    // ── Corsi — per livello ──────────────────────────────────────
    public string[] LivelliLabels   { get; set; } = [];
    public int[]    CorsiPerLivello { get; set; } = [];

    // ── Fornitori ────────────────────────────────────────────────
    public List<FornitoreRow> Fornitori { get; set; } = [];

    public record FornitoreRow(string Nome, string? Email, bool IsGiocatore);
}
