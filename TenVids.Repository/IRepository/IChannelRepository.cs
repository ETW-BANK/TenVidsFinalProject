
using TenVids.Models;

namespace TenVids.Repository.IRepository
{
    public interface IChannelRepository : IRepository<Channel>  
    {
        Task<Channel?> GetByUserIdAsync(string userId, string? includeProperties = null);

    }
}
