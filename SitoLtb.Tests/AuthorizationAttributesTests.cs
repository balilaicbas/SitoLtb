using Microsoft.AspNetCore.Authorization;
using SitoLtb.Areas.Admin.Controllers;

namespace SitoLtb.Tests
{
    public class AuthorizationAttributesTests
    {
        private static AuthorizeAttribute? GetClassAuthorize(Type type) =>
            type.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

        [Fact]
        public void PostController_AllowsAdminAndEditor()
        {
            var attribute = GetClassAuthorize(typeof(PostController));
            Assert.NotNull(attribute);
            Assert.Equal("Admin,Editor", attribute!.Roles);
        }

        [Fact]
        public void DataController_AllowsAdminAndData()
        {
            var attribute = GetClassAuthorize(typeof(DataController));
            Assert.NotNull(attribute);
            Assert.Equal("Admin,Data", attribute!.Roles);
        }
    }
}
