namespace SitoLtb.Data;

public class LtbTipoEvento
{
    public int IdTipoEvento { get; set; }
    public string? Descrizione { get; set; }

    public ICollection<LtbEvento> Eventi { get; set; } = [];
}
