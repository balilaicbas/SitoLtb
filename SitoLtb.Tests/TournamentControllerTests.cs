using Microsoft.AspNetCore.Authorization;
using SitoLtb.Area.Admin.Controllers;

namespace SitoLtb.Tests
{
    public class TournamentControllerTests
    {
        [Fact]
        public void Controller_RequiresAdminRole()
        {
            var attribute = typeof(TournamentController).GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal("Admin", attribute!.Roles);
        }
    }
}
