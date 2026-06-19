using SitoLtb.ViewModels;

namespace SitoLtb.Services
{
    public interface IArchiveService
    {
        Task<ArchiveVM> GetArchiveAsync(int pageEventi = 1, int pageArticoli = 1);
    }
}

