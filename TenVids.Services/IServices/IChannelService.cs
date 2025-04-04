
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IChannelService
    {
        Task<ChannelAddEditVM> GetUserChannelAsync();
        Task<ChannelCreationResult> CreateChannelAsync(ChannelAddEditVM model);
        Task<bool> UserHasChannelAsync();
    }
}
