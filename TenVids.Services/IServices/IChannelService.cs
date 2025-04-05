
using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IChannelService
    {
        Task<ChannelAddEditVM> GetUserChannelAsync();
        Task<ErrorModel<Channel>> CreateChannelAsync(ChannelAddEditVM model);


    }
}
