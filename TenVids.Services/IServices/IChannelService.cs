
using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IChannelService
    {
        Task<ChannelAddEditVM> GetUserChannelAsync();
        Task<ErrorModel<Channel>> CreateChannelAsync(ChannelAddEditVM model);

        Task<ErrorModel<Channel>> UpdateChannelAsync(ChannelAddEditVM model);
        Task DeleteChannelAsync(Channel model);
        Task<Channel> GetChannelByIdAsync(int id);

        Task<ErrorModel<Channel>> Subscribe(int channelId);
    }
}
