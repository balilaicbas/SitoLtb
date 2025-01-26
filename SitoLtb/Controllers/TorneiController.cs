using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using X.PagedList;

namespace SitoLtb.Controllers
{
    public class TorneiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TorneiController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        public async Task<IActionResult> Preiscrizione(int? page)
        {
            
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            var vm = await _context.Tournaments
            .OrderBy(t => t.Data)
            .Where(x => x.Data >= DateTime.Today)
            .ToPagedListAsync(pageNumber, pageSize);
            return View(vm);
        }
       
            public async Task<IActionResult> Blog(int? page)
        {
            var vm = new BlogVM();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.Posts = await _context.Posts!
      .Include(x => x.ApplicationUser)
      .Where(x => x.Categoria == "Tornei")
      .OrderByDescending(x => x.DateTimeCreated)
      .ToPagedListAsync(pageNumber, pageSize);

            return View(vm);
        }
        

        public async Task<IActionResult> Universita(int? page)
        {
            var vm = new BlogVM();
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.Posts = await _context.Posts!
      .Include(x => x.ApplicationUser)
      .Where(x => x.Categoria == "Universita")
      .OrderByDescending(x => x.DateTimeCreated)
      .ToPagedListAsync(pageNumber, pageSize);

            return View(vm);
        }

        public IActionResult Eventi()
        {
            return View();
        }

    }
}
