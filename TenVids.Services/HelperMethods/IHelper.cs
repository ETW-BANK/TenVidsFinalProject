using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.Models;
using TenVids.Models.DTOs;
using TenVids.Models.Pagination;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.HelperMethods
{
    public interface IHelper
    {


        Task<ErrorModel<Videos>> UpdateExistingVideo(
             VideoVM model, byte[] thumbnailBytes, byte[] videoBytes);
        Task<ErrorModel<Videos>> CreateNewVideos(
           VideoVM model, int channelId, byte[] thumbnailBytes, byte[] videoBytes);

        bool IsAcceptableContentType(string type, string contentType);
        string[] AcceptableContentTypes(string type);

        Task<PaginatedList<VideoForHomeDto>> GetVideos(HomeParameters parameters);

        Func<IQueryable<Videos>, IOrderedQueryable<Videos>> GetOrderByExpression(string sortBy);

        Task<byte[]> ProcessUploadedFiles(IFormFile file);

        Task<IEnumerable<SelectListItem>> GetCategoryListAsync();

    }
}
