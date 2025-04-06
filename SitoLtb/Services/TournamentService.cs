using SitoLtb.Data;
using SitoLtb.ViewModels;

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
    }
}
