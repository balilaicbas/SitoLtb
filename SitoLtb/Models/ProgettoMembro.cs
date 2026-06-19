namespace SitoLtb.Models;

public class ProgettoMembro
{
    public int    IdProgetto { get; set; }
    public string UserId     { get; set; } = "";

    public Progetto? Progetto { get; set; }
}
