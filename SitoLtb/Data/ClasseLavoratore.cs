using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class ClasseLavoratore
{
    public int FkidClasse { get; set; }

    public int FkidLavoratore { get; set; }

    public DateOnly Anno { get; set; }

    public virtual Classi FkidClasseNavigation { get; set; } = null!;

    public virtual Lavoratori FkidLavoratoreNavigation { get; set; } = null!;
}
