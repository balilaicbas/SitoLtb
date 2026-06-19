namespace SitoLtb.Data;

public class LtbTipoScuola
{
    public int IdTipoScuola { get; set; }
    public string? Descrizione { get; set; }

    public ICollection<LtbScuola> Scuole { get; set; } = [];
}
