namespace SitoLtb.Data;

public class LtbPartecipanteCorso
{
    public int IdCorso { get; set; }
    public int IdGiocatore { get; set; }

    public LtbCorso? Corso { get; set; }
    public LtbGiocatore? Giocatore { get; set; }
}
