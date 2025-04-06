using SitoLtb.Data;
using SitoLtb.ViewModels;

namespace SitoLtb.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<PostVM> GetAll()
        {
            return _context.Posts!.Select(p => new PostVM
            {
                Title = p.Title,
                CreatedDate = p.DateTimeCreated,
                ThumbnailUrl = p.Image,
                Description = p.Description,


                // altri campi
            }).OrderByDescending(p=>p.CreatedDate).ToList();
        }
    }
}
