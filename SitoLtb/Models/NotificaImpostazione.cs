namespace SitoLtb.Models;

public class NotificaImpostazione
{
    public int     IdNotifica    { get; set; }
    public string  UserId        { get; set; } = "";
    public int     IntervalloOre { get; set; } = 168; // default: settimanale
    public DateTime? UltimoInvio { get; set; }
    public bool    Attiva        { get; set; } = true;
    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;
}
