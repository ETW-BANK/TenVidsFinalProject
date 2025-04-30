using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public interface IMembersService
    {
        Task<MemberVM> GetmembersChannel(int id);
    }
}
