using System;
using System.Collections.Generic;

namespace SitoLtb.Data;

public partial class ControlliRicorrenti
{
    public string Data { get; set; } = null!;

    public string NomeTabella { get; set; } = null!;

    public int Noccorrenze { get; set; }
}
