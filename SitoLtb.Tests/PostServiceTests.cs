using SitoLtb.Models;
using SitoLtb.Services;

namespace SitoLtb.Tests
{
    public class PostServiceTests
    {
        [Fact]
        public void GetAll_ReturnsPostsOrderedByDateDescending()
        {
            var context = TestHelpers.CreateInMemoryContext();
            context.Posts!.AddRange(
                new Post { Id = 1, Title = "Old", Description = "d", Url = "old", Categoria = "c", DateTimeCreated = DateTime.Now.AddDays(-2) },
                new Post { Id = 2, Title = "New", Description = "d", Url = "new", Categoria = "c", DateTimeCreated = DateTime.Now }
            );
            context.SaveChanges();

            var service = new PostService(context);
            var result = service.GetAll();

            Assert.Equal(2, result.Count);
            Assert.Equal("New", result[0].Title);
        }

        [Fact]
        public async Task GetBySlugAsync_ExistingSlug_ReturnsMappedVM()
        {
            var context = TestHelpers.CreateInMemoryContext();
            context.Posts!.Add(new Post { Id = 1, Title = "Hello", Description = "World", Url = "hello-world", Categoria = "c" });
            context.SaveChanges();

            var service = new PostService(context);
            var result = await service.GetBySlugAsync("hello-world");

            Assert.NotNull(result);
            Assert.Equal("Hello", result!.Title);
        }

        [Fact]
        public async Task GetBySlugAsync_MissingSlug_ReturnsNull()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var service = new PostService(context);

            var result = await service.GetBySlugAsync("does-not-exist");

            Assert.Null(result);
        }
    }
}
