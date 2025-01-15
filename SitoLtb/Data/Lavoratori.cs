using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Lavoratori
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public int? Eta { get; set; }

    public string? TipologiaLavoratore { get; set; }

    public string? Provenienza { get; set; }

    public bool? Socio { get; set; }

    public bool? Partecipante { get; set; }

    public bool? Agonista { get; set; }

    public bool? Allievo { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<ClasseLavoratore> ClasseLavoratores { get; set; } = new List<ClasseLavoratore>();

    public virtual ICollection<Eventi> FkidEventos { get; set; } = new List<Eventi>();
}
