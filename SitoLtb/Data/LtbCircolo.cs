namespace SitoLtb.Data;

public class LtbCircolo
{
    public int IdCircolo { get; set; }
    public string RagioneSociale { get; set; } = "";
    public string? Indirizzo { get; set; }
    public string? Localita { get; set; }
    public string? Note { get; set; }

    public ICollection<LtbSocio> Soci { get; set; } = [];
}
