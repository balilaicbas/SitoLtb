using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using X.PagedList;

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

        public async Task<BlogPostVM?> GetBySlugAsync(string slug)
        {
            var post = await _context.Posts!.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Url == slug);
            if (post == null) return null;

            return new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.ApplicationUser is not null
                    ? $"{post.ApplicationUser.FirstName} {post.ApplicationUser.LastName}"
                    : "Autore sconosciuto",
                CreatedDate = post.DateTimeCreated,
                ThumbnailUrl = post.Image,
                Description = post.Description
            };
        }

        public async Task<IPagedList<Post>> GetByCategoriaPagedAsync(string categoria, int pageNumber, int pageSize)
        {
            return await _context.Posts!
                .Where(p => p.Categoria == categoria)
                .OrderByDescending(p => p.DateTimeCreated)
                .ToPagedListAsync(pageNumber, pageSize);
        }
    }
}
