namespace SitoLtb.ViewModels
{
    public class IndexVM
    {
        public List<PostVM> PostsFuturi { get; set; }
        public List<TournamentVM> TournamentsFuturi { get; set; }
        public List<TournamentVM> TorneiProssimoMese { get; set; } = new();
    }
}