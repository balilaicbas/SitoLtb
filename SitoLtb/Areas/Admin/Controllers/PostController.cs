using AspNetCoreHero.ToastNotification.Abstractions;
using SitoLtb.Data;
using SitoLtb.Utilities;
using SitoLtb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SitoLtb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context,
                                INotyfService notyfService,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            

            IQueryable<Post> query = _context.Posts!
                .Include(x => x.ApplicationUser)
                .AsQueryable();  // Mantieni la query come IQueryable

           

            // Ordina la query
            var orderedQuery = query.OrderByDescending(x => x.DateTimeCreated);

            // Pagina la query
            var paginatedList = await orderedQuery
                .Skip((page ?? 1 - 1) * 5)  // Calcola il numero di elementi da saltare
                .Take(5)                    // Prendi i successivi 5 elementi
                .Select(x => new PostVM()   // Trasforma i dati in PostVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    CreatedDate = x.DateTimeCreated,
                    ThumbnailUrl = x.Image,
                    AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName
                })
                .ToListAsync();  // Materializza la lista

            // Creazione della vista con la lista paginata
            return View(paginatedList);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            //get logged in user id
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var post = new Post();

            post.Title = vm.Title;
            post.Description = vm.Description;
            post.ApplicationUserId = loggedInUser!.Id;

            if (post.Title != null)
            {
                string slug = vm.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Url = slug + "-" + Guid.NewGuid();
            }

            if (vm.Thumbnail != null)
            {
                post.Image = UploadImage(vm.Thumbnail);
            }

            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Created Successfully");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
        

            
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            
           
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x=> x.Id == id);
            if(post == null)
            {
                _notification.Error("Post not found");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            
            

            var vm = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ThumbnailUrl = post.Image,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }

            post.Title = vm.Title;
            post.Description = vm.Description;

            if (vm.Thumbnail != null)
            {
                post.Image = UploadImage(vm.Thumbnail);
            }

            await _context.SaveChangesAsync();
            _notification.Success("Post updated succesfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

       


        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using(FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

    }
}
