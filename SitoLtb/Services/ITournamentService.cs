using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Services
{
    public interface ITournamentService
    {
        List<TournamentVM> GetAll();
        Task<IPagedList<TournamentVM>> GetUpcomingBySedePagedAsync(string sede, int pageNumber, int pageSize);
    }
}
