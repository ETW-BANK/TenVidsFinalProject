
using TenVids.Models;

namespace TenVids.Repository.IRepository
{
    public interface IChannelRepository : IRepository<Channel>  
    {
        Task<bool> UserHasChannelAsync(string userId);
    }
}
