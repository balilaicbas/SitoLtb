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

        public List<TournamentVM> GetAllForCalendar()
        {
            return _context.Tournaments
                .OrderBy(t => t.Data)
                .Select(t => new TournamentVM
                {
                    Id                = t.Id,
                    Nome              = t.Nome,
                    Data              = t.Data,
                    Tipologia         = t.Tipologia,
                    Sede              = t.Sede,
                    Elo               = t.Elo,
                    LinkBando         = t.LinkBando,
                    LinkPreiscrizione = t.LinkPreiscrizione
                })
                .ToList();
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

        public List<TournamentVM> GetNextMonth()
        {
            var from = DateTime.Today;
            var to   = DateTime.Today.AddMonths(1);
            return _context.Tournaments!
                .Where(t => t.Data >= from && t.Data <= to)
                .OrderBy(t => t.Data)
                .Select(t => new TournamentVM
                {
                    Nome = t.Nome,
                    Data = t.Data,
                    LinkPreiscrizione = t.LinkPreiscrizione,
                })
                .ToList();
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
