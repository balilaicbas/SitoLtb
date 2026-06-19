namespace SitoLtb.Models;

public class ProgettoNota
{
    public int    IdNota      { get; set; }
    public int    IdProgetto  { get; set; }
    public string? AutoreId   { get; set; }
    public string  Testo      { get; set; } = "";
    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public Progetto? Progetto { get; set; }
}
