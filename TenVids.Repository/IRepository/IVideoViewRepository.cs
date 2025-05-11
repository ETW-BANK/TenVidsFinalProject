using TenVids.Models;

namespace TenVids.Repository.IRepository
{
    public interface IVideoViewRepository: IRepository<VideoViews>   
    {
        Task HandleVideoViewAsync(string userId, int videoId, string ipAddress);
    }
}
