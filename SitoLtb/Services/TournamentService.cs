using SitoLtb.Data;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Services
{
    public class TournamentService:ITournamentService
    {
        private readonly ApplicationDbContext _context;

        public TournamentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TournamentVM> GetAll()
        {
            return _context.Tournaments.Select(t => new TournamentVM
            {
                Nome = t.Nome,
                Data = t.Data,
                LinkBando = t.LinkBando,
                LinkPreiscrizione = t.LinkPreiscrizione,
                // altri campi
            }).OrderByDescending(t => t.Data).ToList();
        }

        public async Task<IPagedList<TournamentVM>> GetUpcomingBySedePagedAsync(string sede, int pageNumber, int pageSize)
        {
            var query = _context.Tournaments!
                .Where(t => t.Data >= DateTime.Today && t.Sede == sede)
                .OrderBy(t => t.Data)
                .Select(t => new TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkBando = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                });

            return await query.ToPagedListAsync(pageNumber, pageSize);
        }
    }
}
