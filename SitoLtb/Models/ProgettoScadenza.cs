namespace SitoLtb.Models;

public class ProgettoScadenza
{
    public int    IdScadenza { get; set; }
    public int    IdProgetto { get; set; }
    public string Titolo     { get; set; } = "";
    public DateOnly Data     { get; set; }
    public string? Nota      { get; set; }

    public Progetto? Progetto { get; set; }
}
