﻿using AspNetCoreHero.ToastNotification.Abstractions;
using SitoLtb.Data;
using SitoLtb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace SitoLtb.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }

        public BlogController(ApplicationDbContext context, INotyfService notification)
        {
            _context = context;
            _notification = notification;
        }
        [HttpGet("[controller]/{slug}")]
        public IActionResult Post(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                _notification.Error("Post non trovato");
                return View();
            }

            var post = _context.Posts!.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Url == slug);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var vm = new BlogPostVM()
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
            return View(vm);
        }
          
        
           
    }
}
