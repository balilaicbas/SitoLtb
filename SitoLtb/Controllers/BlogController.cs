using AspNetCoreHero.ToastNotification.Abstractions;
using SitoLtb.Services;
using Microsoft.AspNetCore.Mvc;


namespace SitoLtb.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostService _postService;
        public INotyfService _notification { get; }

        public BlogController(IPostService postService, INotyfService notification)
        {
            _postService = postService;
            _notification = notification;
        }
        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                _notification.Error("Post non trovato");
                return View();
            }

            var vm = await _postService.GetBySlugAsync(slug);
            if (vm == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            return View(vm);
        }
    }
}
