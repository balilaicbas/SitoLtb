namespace SitoLtb.Data;

public class LtbPartecipanteTorneo
{
    public int IdPartecipanteTornei { get; set; }
    public int IdGiocatore { get; set; }
    public int IdEvento { get; set; }

    public LtbGiocatore? Giocatore { get; set; }
    public LtbEvento? Evento { get; set; }
}
