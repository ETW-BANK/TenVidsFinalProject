
using TenVids.Models;
using TenVids.Models.DTOs;
using TenVids.Models.Pagination;
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
        Task<ErrorModel<Videos>> DeleteVideoAsync(int id);
        Task<IEnumerable<VideoVM>> GetVideosByCategoryIdAsync(int categoryId);
        Task<List<object>> GetVideosByChannelIdAsync(int channelId);

        Task<PaginatedResult<VideoForHomeDto>> GetVideosForHomeGridAsync(HomeParameters parameters);

        Task<PaginatedResult<VideoGridChannelDto>> GetVideosForChannelAsync(BaseParams parameters);

        Task<WatchVideoVM> GetVideoToWatchAsync(int videoId);
        Task<VideoFileDto?> GetVideoFileAsync(int id);
        Task<ErrorModel<VideoFileDto>> DownloadVideoFileAsync(int id);

        Task<ErrorModel<string>> LikeVideo(int videoId, string action, bool like);
        Task<ErrorModel<List<VideoDisplayVm>>> AllVideos();

    }
}
