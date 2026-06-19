namespace SitoLtb.Data;

public class LtbLivello
{
    public int IdLivello { get; set; }
    public string Descrizione { get; set; } = "";

    public ICollection<LtbCorso> Corsi { get; set; } = [];
}
