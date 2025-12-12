using AspNetCoreHero.ToastNotification.Abstractions;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using SitoLtb.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


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
            var listOfPosts = await _context.Posts.ToListAsync();


            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            

            var listOfPostsVM = listOfPosts.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.DateTimeCreated,
                ThumbnailUrl = x.Url,
                Categoria=x.Categoria,
                AuthorName = _userManager.Users
                    .Where(u => u.Id == x.ApplicationUserId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? "Utente sconosciuto" // Gestisce il caso in cui l'utente non venga trovato
            }).ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(await listOfPostsVM.OrderByDescending(x => x.CreatedDate).ToPagedListAsync(pageNumber, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users
         .Select(u => new SelectListItem
         {
             Value = u.Id,
             Text = u.FirstName + " " + u.LastName 
                      })
         .ToListAsync();

            var model = new CreatePostVM();
            ViewBag.Users = users; // Passiamo la lista di utenti alla vista

            return View(model);
          
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(vm);

                // Ottieni l'utente loggato
                var loggedInUser = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

                // ✅ Protezione extra: gestisci utente null
                if (loggedInUser == null)
                {
                    _notification.Error("Utente non autenticato.");
                    return RedirectToAction("Login", "Account"); // o un'altra azione sicura
                }

                var post = new Post
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    Categoria = vm.Categoria,
                    ApplicationUserId = loggedInUser.Id
                };

                // Slug + GUID
                if (!string.IsNullOrWhiteSpace(vm.Title))
                {
                    string slug = vm.Title.Trim().Replace(" ", "-");
                    post.Url = slug + "-" + Guid.NewGuid();
                }

                // Gestione immagine
                if (vm.Thumbnail != null)
                {
                    try
                    {
                        post.Image = UploadImage(vm.Thumbnail); // O async se hai UploadImageAsync
                    }
                    catch (Exception ex)
                    {
                        _notification.Error("Errore nel caricamento dell'immagine.");
                        ModelState.AddModelError("Thumbnail", ex.Message);
                        return View(vm);
                    }
                }

                try
                {
                    await _context.Posts.AddAsync(post);
                    await _context.SaveChangesAsync();
                    _notification.Success("Post Creato");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _notification.Error("Errore di connessione al database: " + ex.Message);
                    return View(vm);
                }
            }
            catch(Exception ex)
            {
                return Content("ERRORE CATTURATO: " + ex.ToString());
            }
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
            post.Categoria = vm.Categoria;

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
            // 1. Controllo sul contenuto
            if (file == null || file.Length == 0)
                throw new ArgumentException("File non valido");

            // 2. Controllo estensione del file
            var estensioniConsentite = new[] {".jpeg",".jpg", ".png"};
            var estensione = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!estensioniConsentite.Contains(estensione))
                throw new ArgumentException("Formato file non supportato");

            // 3. Genera nome univoco
            var nomeFileUnico = Guid.NewGuid().ToString() + estensione;

            // 4. Percorso di destinazione
            var cartella = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(cartella))
                Directory.CreateDirectory(cartella);

            var pathCompleto = Path.Combine(cartella, nomeFileUnico);

            // 5. Salva il file
            using (var stream = new FileStream(pathCompleto, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 6. Ritorna il path relativo (es: da salvare nel DB o usare in un <img>)
            return Path.Combine("uploads", nomeFileUnico).Replace("\\", "/");
        }


    }
}
