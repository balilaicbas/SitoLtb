using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SitoLtb.Areas.Admin.Controllers;
using SitoLtb.Data;
using SitoLtb.Models;

namespace SitoLtb.Tests
{
    public class PostControllerTests
    {
        private static PostController CreateController(out ApplicationDbContext context, IEnumerable<ApplicationUser> users, string loggedInUserName, bool isAdmin)
        {
            context = TestHelpers.CreateInMemoryContext();
            context.Users.AddRange(users);
            context.SaveChanges();

            var userManagerMock = TestHelpers.CreateUserManagerMock();
            userManagerMock.Setup(m => m.Users).Returns(context.Users);

            var controller = new PostController(
                context,
                Mock.Of<INotyfService>(),
                Mock.Of<IWebHostEnvironment>(),
                userManagerMock.Object,
                Mock.Of<ILogger<PostController>>());

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, loggedInUserName) };
            if (isAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
        }

        [Fact]
        public async Task Delete_NonOwnerNonAdmin_ReturnsForbid()
        {
            var owner = new ApplicationUser { Id = "owner-1", UserName = "owner" };
            var editor = new ApplicationUser { Id = "editor-1", UserName = "editor" };

            var controller = CreateController(out var context, new[] { owner, editor }, "editor", isAdmin: false);
            var post = new Post { Id = 1, Title = "t", Description = "d", Url = "u", Categoria = "c", ApplicationUserId = owner.Id };
            context.Posts!.Add(post);
            await context.SaveChangesAsync();

            var result = await controller.Delete(1);

            Assert.IsType<ForbidResult>(result);
            Assert.NotNull(context.Posts!.Find(1));
        }

        [Fact]
        public async Task Delete_Owner_DeletesSuccessfully()
        {
            var owner = new ApplicationUser { Id = "owner-1", UserName = "owner" };

            var controller = CreateController(out var context, new[] { owner }, "owner", isAdmin: false);
            var post = new Post { Id = 2, Title = "t", Description = "d", Url = "u", Categoria = "c", ApplicationUserId = owner.Id };
            context.Posts!.Add(post);
            await context.SaveChangesAsync();

            var result = await controller.Delete(2);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(context.Posts!.Find(2));
        }

        [Fact]
        public async Task Delete_Admin_DeletesAnyonesPost()
        {
            var owner = new ApplicationUser { Id = "owner-1", UserName = "owner" };
            var admin = new ApplicationUser { Id = "admin-1", UserName = "admin" };

            var controller = CreateController(out var context, new[] { owner, admin }, "admin", isAdmin: true);
            var post = new Post { Id = 3, Title = "t", Description = "d", Url = "u", Categoria = "c", ApplicationUserId = owner.Id };
            context.Posts!.Add(post);
            await context.SaveChangesAsync();

            var result = await controller.Delete(3);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(context.Posts!.Find(3));
        }
    }
}
