using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IHomeService
    {
        Task<HomeVM> GoToHomeAsync(string page);
    }
}
