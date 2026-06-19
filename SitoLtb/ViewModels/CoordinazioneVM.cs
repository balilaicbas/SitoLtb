using SitoLtb.Models;

namespace SitoLtb.ViewModels;

public class ProgettoCardVM
{
    public int         IdProgetto   { get; set; }
    public string      Titolo       { get; set; } = "";
    public string?     Descrizione  { get; set; }
    public DateOnly    DataInizio   { get; set; }
    public DateOnly?   DataScadenza { get; set; }
    public string      Colore       { get; set; } = "#4e73df";
    public StatoProgetto Stato      { get; set; }
    public string?     ReferenteNome { get; set; }
    public int         TotaleTask   { get; set; }
    public int         TaskRisolte  { get; set; }
    public int         NumMembri    { get; set; }
    public int         NumScadenze  { get; set; }
    public int         NumNote      { get; set; }

    public int Avanzamento => TotaleTask == 0 ? 0 : (int)Math.Round(TaskRisolte * 100.0 / TotaleTask);

    public bool IsScaduto => DataScadenza.HasValue && DataScadenza.Value < DateOnly.FromDateTime(DateTime.Today);
    public bool IsInScadenza => DataScadenza.HasValue
        && DataScadenza.Value >= DateOnly.FromDateTime(DateTime.Today)
        && DataScadenza.Value <= DateOnly.FromDateTime(DateTime.Today.AddDays(7));
}

public class CoordinazioneIndexVM
{
    public List<ProgettoCardVM> Progetti { get; set; } = [];
    public int TotaleProgetti  { get; set; }
    public int ProgettiAttivi  { get; set; }
    public int ProgettiCompletati { get; set; }
    public int ProgettiSospesi { get; set; }
}

public class TaskCommentoVM
{
    public int    IdCommento   { get; set; }
    public string AutoreNome   { get; set; } = "";
    public string Testo        { get; set; } = "";
    public DateTime DataCreazione { get; set; }
}

public class ProgettoTaskVM
{
    public int      IdTask       { get; set; }
    public int      IdProgetto   { get; set; }
    public string   Titolo       { get; set; } = "";
    public string?  Descrizione  { get; set; }
    public StatoTask Stato       { get; set; }
    public PrioritaTask Priorita { get; set; }
    public DateTime? DataScadenza { get; set; }
    public string?  AssegnatoAId  { get; set; }
    public string?  AssegnatoANome { get; set; }
    public DateTime DataCreazione { get; set; }
    public DateTime? DataCompletamento { get; set; }
    public List<TaskCommentoVM> Commenti { get; set; } = [];
}

public class MembroVM
{
    public string UserId { get; set; } = "";
    public string Nome   { get; set; } = "";
    public string Email  { get; set; } = "";
}

public class ProgettoDetailVM
{
    public int         IdProgetto   { get; set; }
    public string      Titolo       { get; set; } = "";
    public string?     Descrizione  { get; set; }
    public DateOnly    DataInizio   { get; set; }
    public DateOnly?   DataScadenza { get; set; }
    public string      Colore       { get; set; } = "#4e73df";
    public StatoProgetto Stato      { get; set; }
    public string?     ReferenteId  { get; set; }
    public string?     ReferenteNome { get; set; }

    public List<ProgettoTaskVM>    Tasks    { get; set; } = [];
    public List<ProgettoNota>      Note     { get; set; } = [];
    public List<ProgettoScadenza>  Scadenze { get; set; } = [];
    public List<MembroVM>          Membri   { get; set; } = [];

    public List<MembroVM>          UtentiDisponibili { get; set; } = [];

    public int Avanzamento => Tasks.Count == 0 ? 0
        : (int)Math.Round(Tasks.Count(t => t.Stato == StatoTask.Risolto) * 100.0 / Tasks.Count);
}

public class NotificaVM
{
    public int      IdNotifica    { get; set; }
    public string   UserId        { get; set; } = "";
    public string   NomeUtente    { get; set; } = "";
    public int      IntervalloOre { get; set; }
    public DateTime? UltimoInvio  { get; set; }
    public bool     Attiva        { get; set; }

    public string IntervalloLabel => IntervalloOre switch {
        24   => "Ogni giorno",
        48   => "Ogni 2 giorni",
        72   => "Ogni 3 giorni",
        168  => "Ogni settimana",
        336  => "Ogni 2 settimane",
        720  => "Ogni mese",
        _    => $"Ogni {IntervalloOre}h"
    };
}

public class ImpostazioniNotificheVM
{
    public List<NotificaVM> Impostazioni      { get; set; } = [];
    public List<MembroVM>   UtentiDisponibili { get; set; } = [];
}

public class CreaProgettoVM
{
    public string  Titolo       { get; set; } = "";
    public string? Descrizione  { get; set; }
    public DateOnly DataInizio  { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? DataScadenza { get; set; }
    public string? ReferenteId  { get; set; }
    public string  Colore       { get; set; } = "#4e73df";
    public StatoProgetto Stato  { get; set; } = StatoProgetto.Attivo;
    public List<MembroVM> UtentiDisponibili { get; set; } = [];
}
