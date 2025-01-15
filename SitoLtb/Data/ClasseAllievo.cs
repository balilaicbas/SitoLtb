using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class ClasseAllievo
{
    public int FkidClasse { get; set; }

    public int FkidAllievo { get; set; }

    public DateOnly Anno { get; set; }

    public virtual Allievi FkidAllievoNavigation { get; set; } = null!;

    public virtual Classi FkidClasseNavigation { get; set; } = null!;
}
