using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Poli
{
    public int Id { get; set; }

    public string Piva { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string? Indirizzo { get; set; }

    public virtual ICollection<Agonisti> Agonistis { get; set; } = new List<Agonisti>();

    public virtual ICollection<Cooperazioni> Cooperazionis { get; set; } = new List<Cooperazioni>();

    public virtual ICollection<PoloScuola> PoloScuolas { get; set; } = new List<PoloScuola>();

    public virtual ICollection<SocioPolo> SocioPolos { get; set; } = new List<SocioPolo>();

    public virtual ICollection<Eventi> FkidEventos { get; set; } = new List<Eventi>();
}
