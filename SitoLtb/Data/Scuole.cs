using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Scuole
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Indirizzo { get; set; } = null!;

    public string? TipologiaScuola { get; set; }

    public string? Telefono { get; set; }

    public string? Mail { get; set; }

    public virtual ICollection<Classi> Classis { get; set; } = new List<Classi>();

    public virtual ICollection<PoloScuola> PoloScuolas { get; set; } = new List<PoloScuola>();
}
