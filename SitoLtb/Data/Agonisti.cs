using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class Agonisti
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public string IdFsi { get; set; } = null!;

    public string IdFide { get; set; } = null!;

    public DateOnly? AnnoPrimaIscrizione { get; set; }

    public string? TipoTessera { get; set; }

    public bool? Socio { get; set; }

    public bool? Partecipante { get; set; }

    public bool? Lavoratore { get; set; }

    public bool? Allievo { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public int FkidPolo { get; set; }

    public virtual Poli FkidPoloNavigation { get; set; } = null!;
}
