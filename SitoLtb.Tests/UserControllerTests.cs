using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SitoLtb.Areas.Admin.Controllers;
using SitoLtb.Models;
using SitoLtb.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SitoLtb.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void Delete_RequiresAdminRole()
        {
            var method = typeof(UserController).GetMethod(nameof(UserController.Delete), new[] { typeof(string) });
            var attribute = method!.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal("Admin", attribute!.Roles);
        }

        private static (UserController controller, Mock<SignInManager<ApplicationUser>> signInManagerMock) CreateController()
        {
            var userManagerMock = TestHelpers.CreateUserManagerMock();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            var controller = new UserController(
                userManagerMock.Object,
                signInManagerMock.Object,
                Mock.Of<INotyfService>(),
                Mock.Of<ILogger<UserController>>());

            return (controller, signInManagerMock);
        }

        [Fact]
        public async Task Login_LockedOut_ReturnsViewWithoutRedirect()
        {
            var (controller, signInManagerMock) = CreateController();
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            var result = await controller.Login(new LoginVM { Username = "user", Password = "wrong" });

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Success_RedirectsToPostIndex()
        {
            var (controller, signInManagerMock) = CreateController();
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var result = await controller.Login(new LoginVM { Username = "user", Password = "correct" });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Post", redirect.ControllerName);
        }
    }
}
