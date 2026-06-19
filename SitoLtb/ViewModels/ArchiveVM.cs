using X.PagedList;

namespace SitoLtb.ViewModels
{
    public class ArchiveVM
    {
        public IPagedList<PostVM>? PostsPassati { get; set; }
        public IPagedList<TournamentVM>? TournamentsPassati { get; set; }
    }
}