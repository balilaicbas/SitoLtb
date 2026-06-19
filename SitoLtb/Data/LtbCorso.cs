namespace SitoLtb.Data;

public class LtbCorso
{
    public int IdCorso { get; set; }
    public int IdLivello { get; set; }
    public string Descrizione { get; set; } = "";
    public DateOnly DataInizio { get; set; }
    public DateOnly DataFine { get; set; }
    public int IdScuola { get; set; }

    public LtbLivello? Livello { get; set; }
    public LtbScuola? Scuola { get; set; }
    public ICollection<LtbPartecipanteCorso> Partecipanti { get; set; } = [];
}
