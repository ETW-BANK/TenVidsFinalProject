
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IVideosService
    {
        Task<IEnumerable<VideoVM>> GetAllVideosAsync();
        Task<VideoVM> GetVideoByIdAsync(int? id);
        Task<bool> UserHasChannelAsync();
        Task<ErrorModel<Videos>> CreateEditVideoAsync(VideoVM model);
        Task<ErrorModel<Videos>> UpdateVideoAsync(VideoVM model);
        Task DeleteVideoAsync(Videos video);
        Task<IEnumerable<VideoVM>> GetVideosByCategoryIdAsync(int categoryId);
        Task<IEnumerable<VideoVM>> GetVideosByChannelIdAsync(int channelId);
       
    }
}
