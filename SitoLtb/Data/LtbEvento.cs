namespace SitoLtb.Data;

public class LtbEvento
{
    public int IdEvento { get; set; }
    public string Descrizione { get; set; } = "";
    public int IdTipoEvento { get; set; }
    public DateOnly DataInizio { get; set; }
    public DateOnly DataFine { get; set; }

    public LtbTipoEvento? TipoEvento { get; set; }
    public ICollection<LtbPartecipanteTorneo> Partecipanti { get; set; } = [];
}
