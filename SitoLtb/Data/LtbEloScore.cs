namespace SitoLtb.Data;

public class LtbEloScore
{
    public int IdScore { get; set; }
    public int IdGiocatore { get; set; }
    public DateOnly DataAcquisizione { get; set; }
    public int? EloStandard { get; set; }
    public int? EloRapid { get; set; }
    public int? EloBlitz { get; set; }

    public LtbGiocatore? Giocatore { get; set; }
}
