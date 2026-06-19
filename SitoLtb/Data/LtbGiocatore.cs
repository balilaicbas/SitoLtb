namespace SitoLtb.Data;

public class LtbGiocatore
{
    public int IdGiocatore { get; set; }
    public int? IdFide { get; set; }
    public int? IdFSI { get; set; }
    public int? IdNazionalita { get; set; }
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public string? Indirizzo { get; set; }
    public DateOnly? DataNascita { get; set; }
    public string? Email { get; set; }
    public string? NumeroCivico { get; set; }
    public int? CAP { get; set; }
    public string? Localita { get; set; }

    public ICollection<LtbEloScore> EloScores { get; set; } = [];
    public ICollection<LtbSocio> Soci { get; set; } = [];
    public ICollection<LtbPartecipanteTorneo> Partecipazioni { get; set; } = [];
    public ICollection<LtbPartecipanteCorso> PartecipantiCorsi { get; set; } = [];
}
