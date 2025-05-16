using TenVids.Models;

namespace TenVids.Data.Access.IRepo
{
    public interface IVideoViewRepository: IRepository<VideoViews>   
    {
        Task HandleVideoViewAsync(string userId, int videoId, string ipAddress);
    }
}
