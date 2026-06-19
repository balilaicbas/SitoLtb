using SitoLtb.Models;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Services
{
    public interface IPostService
    {
        List<PostVM> GetAll();
        Task<BlogPostVM?> GetBySlugAsync(string slug);
        Task<IPagedList<Post>> GetByCategoriaPagedAsync(string categoria, int pageNumber, int pageSize);
        Task<List<PostVM>> GetRelatedAsync(int postId, string categoria, string title, int count = 5);
    }

}
