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
            return _context.Posts!
                .Select(p => new PostVM
                {
                    Title = p.Title,
                    AuthorName = p.ApplicationUser!.FirstName + " " + p.ApplicationUser.LastName,
                    CreatedDate = p.DateTimeCreated,
                    ThumbnailUrl = p.Image,
                    Description = p.Description,
                    Url = p.Url,
                })
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }

        public async Task<BlogPostVM?> GetBySlugAsync(string slug)
        {
            var post = await _context.Posts!.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Url == slug);
            if (post == null) return null;

            return new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                Categoria = post.Categoria,
                AuthorName = post.ApplicationUser is not null
                    ? $"{post.ApplicationUser.FirstName} {post.ApplicationUser.LastName}"
                    : "Autore sconosciuto",
                CreatedDate = post.DateTimeCreated,
                ThumbnailUrl = post.Image,
                Description = post.Description,
                Url = post.Url,
            };
        }

        public async Task<List<PostVM>> GetRelatedAsync(int postId, string categoria, string title, int count = 5)
        {
            var candidates = await _context.Posts!
                .Where(p => p.Id != postId && p.Categoria == categoria)
                .OrderByDescending(p => p.DateTimeCreated)
                .Select(p => new PostVM
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorName = p.ApplicationUser!.FirstName + " " + p.ApplicationUser.LastName,
                    CreatedDate = p.DateTimeCreated,
                    ThumbnailUrl = p.Image,
                    Url = p.Url,
                })
                .ToListAsync();

            var titleWords = title.ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length >= 3)
                .ToHashSet();

            return candidates
                .OrderByDescending(p =>
                    (p.Title ?? "").ToLowerInvariant()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Count(w => w.Length >= 3 && titleWords.Contains(w)))
                .ThenByDescending(p => p.CreatedDate)
                .Take(count)
                .ToList();
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
