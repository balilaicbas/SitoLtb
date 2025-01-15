using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Classi
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateOnly Data { get; set; }

    public string Argomento { get; set; } = null!;

    public int FkidScuola { get; set; }

    public virtual ICollection<ClasseAllievo> ClasseAllievos { get; set; } = new List<ClasseAllievo>();

    public virtual ICollection<ClasseLavoratore> ClasseLavoratores { get; set; } = new List<ClasseLavoratore>();

    public virtual Scuole FkidScuolaNavigation { get; set; } = null!;
}
