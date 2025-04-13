
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
        Task<IEnumerable<VideoVM>> GetVideosByChannelIdAsync(int channelId);

        //Task<PaginatedList<VideoGridChannelDto>> GetVideosForChannelGrid(int channelId, BaseParams parameters);

        Task<PaginatedResult<VideoGridChannelDto>> GetVideosForChannelAsync(BaseParams parameters);

    }
}
