using SitoLtb.Data;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Services
{
    public class ArchiveService : IArchiveService
    {
        private const int PageSize = 10;
        private readonly ApplicationDbContext _context;

        public ArchiveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ArchiveVM> GetArchiveAsync(int pageEventi = 1, int pageArticoli = 1)
        {
            return new ArchiveVM
            {
                PostsPassati = await _context.Posts!
                    .Where(p => p.DateTimeCreated < DateTime.Today)
                    .OrderByDescending(p => p.DateTimeCreated)
                    .Select(p => new PostVM
                    {
                        Title = p.Title,
                        Categoria = p.Categoria,
                        AuthorName = p.ApplicationUser!.FirstName + " " + p.ApplicationUser.LastName,
                        CreatedDate = p.DateTimeCreated,
                        ThumbnailUrl = p.Image!,
                        Description = p.Description,
                        Url = p.Url,
                    })
                    .ToPagedListAsync(pageArticoli, PageSize),

                TournamentsPassati = await _context.Tournaments!
                    .Where(t => t.Data < DateTime.Today)
                    .OrderByDescending(t => t.Data)
                    .Select(t => new TournamentVM
                    {
                        Nome = t.Nome,
                        Data = t.Data,
                        Tipologia = t.Tipologia,
                        Sede = t.Sede,
                        Elo = t.Elo,
                        LinkBando = t.LinkBando,
                        LinkPreiscrizione = t.LinkPreiscrizione,
                    })
                    .ToPagedListAsync(pageEventi, PageSize),
            };
        }
    }
}