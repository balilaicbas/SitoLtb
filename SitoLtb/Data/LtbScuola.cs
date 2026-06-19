namespace SitoLtb.Data;

public class LtbScuola
{
    public int IdScuola { get; set; }
    public string Nome { get; set; } = "";
    public string? Descrizione { get; set; }
    public int IdTipoScuola { get; set; }

    public LtbTipoScuola? TipoScuola { get; set; }
    public ICollection<LtbCorso> Corsi { get; set; } = [];
}
