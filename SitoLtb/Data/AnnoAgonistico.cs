using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class AnnoAgonistico
{
    public int Anno { get; set; }

    public int FkidAgonista { get; set; }

    public int? NTorneiSvolti { get; set; }

    public int? VariazioneElo { get; set; }

    public int? Elostandard1gen { get; set; }

    public int? EloRapid1gen { get; set; }

    public int? EloBlitz1gen { get; set; }

    public int? EloStandard1mar { get; set; }

    public int? EloRapid1mar { get; set; }

    public int? EloBlitz1mar { get; set; }

    public int? EloStandard1giu { get; set; }

    public int? EloRapid1giu { get; set; }

    public int? EloBlitz1giu { get; set; }

    public int? EloStandard1set { get; set; }

    public int? EloRapid1set { get; set; }

    public int? EloBlitz1set { get; set; }

    public virtual Agonisti FkidAgonistaNavigation { get; set; } = null!;
}
