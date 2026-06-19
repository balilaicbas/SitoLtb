namespace SitoLtb.Models;

public class ProgettoTask
{
    public int    IdTask      { get; set; }
    public int    IdProgetto  { get; set; }
    public string Titolo      { get; set; } = "";
    public string? Descrizione { get; set; }
    public StatoTask    Stato    { get; set; } = StatoTask.InCorso;
    public PrioritaTask Priorita { get; set; } = PrioritaTask.Media;
    public DateTime? DataScadenza      { get; set; }
    public string?   AssegnatoAId      { get; set; }
    public DateTime  DataCreazione     { get; set; } = DateTime.UtcNow;
    public DateTime? DataCompletamento { get; set; }

    public Progetto? Progetto { get; set; }
    public ICollection<TaskCommento> Commenti { get; set; } = [];
}

public enum StatoTask    { InCorso = 0, Risolto = 1 }
public enum PrioritaTask { Bassa = 0, Media = 1, Alta = 2 }
