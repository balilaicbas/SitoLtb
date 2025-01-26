using SitoLtb.Models;
using X.PagedList;

namespace SitoLtb.ViewModels
{
    public class BlogVM
    {
        public string? Title { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IPagedList<Post>? Posts { get; set; }
    }
}
