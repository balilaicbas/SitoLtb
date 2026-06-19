namespace SitoLtb.Data;

public class LtbFornitore
{
    public int IdFornitore { get; set; }
    public int? IdGiocatore { get; set; }
    public string RagioneSociale { get; set; } = "";
    public string? Indirizzo { get; set; }
    public string? NumeroCivico { get; set; }
    public string? Email { get; set; }
}
