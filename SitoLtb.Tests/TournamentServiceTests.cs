using SitoLtb.Models;
using SitoLtb.Services;

namespace SitoLtb.Tests
{
    public class TournamentServiceTests
    {
        [Fact]
        public void GetAll_ReturnsTournamentsOrderedByDateDescending()
        {
            var context = TestHelpers.CreateInMemoryContext();
            context.Tournaments.AddRange(
                new Tournament { Id = 1, Nome = "Old", Tipologia = "t", Sede = "Verdolina", LinkBando = "", LinkPreiscrizione = "", Url = "old", Data = DateTime.Today.AddDays(-10) },
                new Tournament { Id = 2, Nome = "New", Tipologia = "t", Sede = "Verdolina", LinkBando = "", LinkPreiscrizione = "", Url = "new", Data = DateTime.Today.AddDays(10) }
            );
            context.SaveChanges();

            var service = new TournamentService(context);
            var result = service.GetAll();

            Assert.Equal(2, result.Count);
            Assert.Equal("New", result[0].Nome);
        }

        [Fact]
        public async Task GetUpcomingBySedePagedAsync_FiltersBySedeAndFutureDate()
        {
            var context = TestHelpers.CreateInMemoryContext();
            context.Tournaments.AddRange(
                new Tournament { Id = 1, Nome = "PastVerdolina", Tipologia = "t", Sede = "Verdolina", LinkBando = "", LinkPreiscrizione = "", Url = "p", Data = DateTime.Today.AddDays(-1) },
                new Tournament { Id = 2, Nome = "FutureVerdolina", Tipologia = "t", Sede = "Verdolina", LinkBando = "", LinkPreiscrizione = "", Url = "f", Data = DateTime.Today.AddDays(1) },
                new Tournament { Id = 3, Nome = "FutureComala", Tipologia = "t", Sede = "Comala", LinkBando = "", LinkPreiscrizione = "", Url = "c", Data = DateTime.Today.AddDays(1) }
            );
            context.SaveChanges();

            var service = new TournamentService(context);
            var result = await service.GetUpcomingBySedePagedAsync("Verdolina", pageNumber: 1, pageSize: 10);

            Assert.Single(result);
            Assert.Equal("FutureVerdolina", result[0].Nome);
        }
    }
}
