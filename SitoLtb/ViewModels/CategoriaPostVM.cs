using X.PagedList;
using SitoLtb.Models;

namespace SitoLtb.ViewModels
{
    public class CategoriaPostVM
    {
        public string Titolo { get; set; } = "";
        public IPagedList<Post>? Posts { get; set; }
    }
}
