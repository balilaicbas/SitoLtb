using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Cooperazioni
{
    public int Id { get; set; }

    public DateOnly DataInizioCooperazione { get; set; }

    public DateOnly? DataFineCooperazione { get; set; }

    public string? Tipologia { get; set; }

    public int FkpoloId { get; set; }

    public int FkrealtaEsternaId { get; set; }

    public virtual Poli Fkpolo { get; set; } = null!;

    public virtual RealtaEsterne FkrealtaEsterna { get; set; } = null!;
}
