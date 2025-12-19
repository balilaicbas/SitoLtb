using X.PagedList;

namespace SitoLtb.ViewModels
{
    public class PreiscrizioneVM
    {
        public IPagedList<TournamentVM> Verdolina { get; set; } = default!;
        public IPagedList<TournamentVM> Comala { get; set; } = default!;
    }
}
