using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Soci
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public int? Eta { get; set; }

    public string TipologiaIscrizione { get; set; } = null!;

    public int? Elo { get; set; }

    public DateOnly? AnnoPrimaIscrizione { get; set; }

    public bool? Partecipante { get; set; }

    public bool? Agonista { get; set; }

    public bool? Lavoratore { get; set; }

    public bool? Allievo { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<SocioPolo> SocioPolos { get; set; } = new List<SocioPolo>();
}
