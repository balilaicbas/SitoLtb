namespace SitoLtb.Models;

public class TaskCommento
{
    public int    IdCommento    { get; set; }
    public int    IdTask        { get; set; }
    public string? AutoreId     { get; set; }
    public string  Testo        { get; set; } = "";
    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public ProgettoTask? Task { get; set; }
}
