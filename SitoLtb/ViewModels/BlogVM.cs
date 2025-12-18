using SitoLtb.Models;
using X.PagedList;

namespace SitoLtb.ViewModels
{
    public class BlogVM
    {
        public IPagedList<Post>? InEvidenza { get; set; }
        public IPagedList<Post>? Tornei { get; set; }
        public IPagedList<Post>? Eventi { get; set; }
        public IPagedList<Post>? Cis { get; set; }
    }
}
