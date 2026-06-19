namespace SitoLtb.Models;

public class Progetto
{
    public int    IdProgetto   { get; set; }
    public string Titolo       { get; set; } = "";
    public string? Descrizione { get; set; }
    public DateOnly  DataInizio  { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public string? ReferenteId  { get; set; }
    public string  Colore       { get; set; } = "#4e73df";
    public StatoProgetto Stato  { get; set; } = StatoProgetto.Attivo;
    public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

    public ICollection<ProgettoTask>     Tasks    { get; set; } = [];
    public ICollection<ProgettoNota>     Note     { get; set; } = [];
    public ICollection<ProgettoScadenza> Scadenze { get; set; } = [];
    public ICollection<ProgettoMembro>   Membri   { get; set; } = [];
}

public enum StatoProgetto { Attivo = 0, Completato = 1, Sospeso = 2 }
