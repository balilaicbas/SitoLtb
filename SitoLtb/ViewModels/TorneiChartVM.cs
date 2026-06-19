namespace SitoLtb.ViewModels;

public class TorneiChartVM
{
    public string[] Labels { get; set; } = [];
    public int[] Counts { get; set; } = [];

    public int TotaleTornei => Labels.Length;
    public int TotalePartecipanti => Counts.Length > 0 ? Counts.Sum() : 0;
    public double Media => TotaleTornei > 0 ? Math.Round((double)TotalePartecipanti / TotaleTornei, 1) : 0;
    public int Massimo => Counts.Length > 0 ? Counts.Max() : 0;
}
