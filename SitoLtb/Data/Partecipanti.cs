using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Partecipanti
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public int? Eta { get; set; }

    public bool? PrimoEvento { get; set; }

    public string? Circolo { get; set; }

    public int? Elo { get; set; }

    public bool? Socio { get; set; }

    public bool? Agonista { get; set; }

    public bool? Lavoratore { get; set; }

    public bool? Allievo { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Eventi> FkidEventos { get; set; } = new List<Eventi>();
}
