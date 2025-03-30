
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IChannelService
    {
        Task<ChannelAddEditVM> GetUserChannelAsync();
        Task CreateChannelAsync(ChannelAddEditVM model);
        Task<bool> UserHasChannelAsync();
    }
}
