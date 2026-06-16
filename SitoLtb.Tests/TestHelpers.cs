using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SitoLtb.Data;
using SitoLtb.Models;

namespace SitoLtb.Tests
{
    public static class TestHelpers
    {
        public static ApplicationDbContext CreateInMemoryContext(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
