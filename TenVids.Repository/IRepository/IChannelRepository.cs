
using TenVids.Models;

namespace TenVids.Repository.IRepository
{
    public interface IChannelRepository : IRepository<Channel>  
    {
        Task<Channel?> GetByUserIdAsync(string userId, string? includeProperties = null);
        Task CreateAsync(Channel channel);
        Task<bool> UserHasChannelAsync(string userId);
    }
}
