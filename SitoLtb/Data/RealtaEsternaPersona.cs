using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class RealtaEsternaPersona
{
    public int FkidRealtaEsterna { get; set; }

    public int FkidPersona { get; set; }

    public DateOnly Anno { get; set; }

    public virtual Persone FkidPersonaNavigation { get; set; } = null!;

    public virtual RealtaEsterne FkidRealtaEsternaNavigation { get; set; } = null!;
}
