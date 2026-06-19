namespace SitoLtb.Data;

public class LtbSocio
{
    public int IdSocio { get; set; }
    public int IdGiocatore { get; set; }
    public int IdCircolo { get; set; }
    public int Anno { get; set; }

    public LtbGiocatore? Giocatore { get; set; }
    public LtbCircolo? Circolo { get; set; }
}
