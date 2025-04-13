using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TenVids.Utilities.FileHelpers;
using TenVids.Models.Pagination;
using TenVids.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace TenVids.Services
{
    public class VideosService: IVideosService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly FileUploadConfig _fileUploadConfig;
        private readonly IPicService _picService;
      

        public VideosService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptions<FileUploadConfig> fileUploadConfig,IPicService picService,ILogger<VideosService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _picService = picService;
            _configuration = configuration;
            _fileUploadConfig = fileUploadConfig.Value;
            
        }

        public async Task<ErrorModel<Videos>> CreateEditVideoAsync(VideoVM model)
        {
            // Null check for HttpContext
            if (_httpContextAccessor?.HttpContext == null)
            {
                return ErrorModel<Videos>.Failure("HttpContext not available", 500);
            }

            // Get user channel
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userChannel = await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x => x.AppUserId == userId);

            if (userChannel == null)
            {
                return ErrorModel<Videos>.Failure("No Channel Found With Your Account", 404);
            }

            // Validate for new videos
            if (model.Id == 0)
            {
                var videoExists = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                    x => x.Title == model.Title && x.ChannelId == userChannel.Id);

                if (videoExists != null)
                {
                    return ErrorModel<Videos>.Failure("Video with this title already exists", 409);
                }
            }

            if (model.ImageUpload == null)
                return ErrorModel<Videos>.Failure("Image thumbnail is required", 400);

            if (!IsAcceptableContentType("image", model.ImageUpload.ContentType))
            {
                var allowedTypes = AcceptableContentTypes("image");
                return ErrorModel<Videos>.Failure(
                    $"Invalid image type. Allowed: {string.Join(", ", allowedTypes)}",
                    400);
            }

            if (model.ImageUpload.Length > _fileUploadConfig.ImageMaxSizeInMB * SD.fileSizeLimit)
                return ErrorModel<Videos>.Failure(
                    $"Image exceeds maximum size of {_fileUploadConfig.ImageMaxSizeInMB}MB",
                    400);

            if (model.VideoUpload == null)
                return ErrorModel<Videos>.Failure("Video file is required", 400);

            if (!IsAcceptableContentType("video", model.VideoUpload.ContentType))
            {
                var allowedTypes = AcceptableContentTypes("video");
                return ErrorModel<Videos>.Failure(
                    $"Invalid video type. Allowed: {string.Join(", ", allowedTypes)}",
                    400);
            }

            if (model.VideoUpload.Length > _fileUploadConfig.VideoMaxSizeInMB * SD.fileSizeLimit)
                return ErrorModel<Videos>.Failure(
                    $"Video exceeds maximum size of {_fileUploadConfig.VideoMaxSizeInMB}MB",
                    400);

            // Process uploaded files
            var thumbnailBytes = await ProcessUploadedFiles(model.ImageUpload);
            var videoBytes = await ProcessUploadedFiles(model?.VideoUpload);


            if (model.Id == 0)
            {
                return await CreateNewVideos(model, userChannel.Id, thumbnailBytes, videoBytes);
            }
            else
            {
                return await UpdateExistingVideo(model, thumbnailBytes, videoBytes);
            }
        }

        public async Task<PaginatedResult<VideoGridChannelDto>> GetVideosForChannelAsync(BaseParams parameters)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Single optimized query
            var query = from channel in _unitOfWork.ChannelRepository.GetQueryable()
                        where channel.AppUserId == userId
                        join video in _unitOfWork.VideosRepository.GetQueryable()
                            on channel.Id equals video.ChannelId
                        select new VideoGridChannelDto
                        {
                            Id = video.Id,
                            ThumbnailUrl = video.Thumbnail,
                            Title = video.Title,
                            CreatedAt = video.CreatedAt,
                            CategoryName = video.Category.Name,
                            Views = SD.GetRandomNumber(10000, 500000, video.Id),
                            Comments = SD.GetRandomNumber(1, 100, video.Id),
                            Likes = SD.GetRandomNumber(5, 100, video.Id),
                            Dislikes = SD.GetRandomNumber(5, 100, video.Id)
                        };

            // Apply sorting
            query = parameters.SortBy switch
            {
                "title-a" => query.OrderBy(x => x.Title),
                "title-d" => query.OrderByDescending(x => x.Title),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            // Single roundtrip for count + data
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<VideoGridChannelDto>(
                items,
                totalCount,
                parameters.PageSize,
                parameters.PageNumber,
                (int)Math.Ceiling(totalCount / (double)parameters.PageSize));
        }

        public async Task<ErrorModel<Videos>> DeleteVideoAsync(int id)
        {
   
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetUserId();

                var existingVideo = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                    x => x.Id == id && x.Channel.AppUserId == userId,
                    includeProperties: "Channel");

                if (existingVideo == null)
                {

                    return ErrorModel<Videos>.Failure("Video not found or you don't have permission", 404);
                }

                _unitOfWork.VideosRepository.Remove(existingVideo);
                await _unitOfWork.CompleteAsync();

                return ErrorModel<Videos>.Success(existingVideo, $"Video '{existingVideo.Title}' deleted successfully");
            }
            catch (Exception ex)
            {
             
                return ErrorModel<Videos>.Failure("An error occurred while deleting the video", 500);
            }
        }
        public async Task<IEnumerable<VideoVM>> GetAllVideosAsync()
        {
            var result = await _unitOfWork.VideosRepository.GetAllAsync(
                               includeProperties: "Category,Channel",
                                              orderby: GetOrderByExpression("date-d"));

            return result.Select(video => new VideoVM
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                CategoryId = video.CategoryId,
                ImageUrl = video.Thumbnail,

            });
        }
        public async Task<VideoVM> GetVideoByIdAsync(int? id)
        {
            if (!await UserHasChannelAsync())
            {
                return null;
            }

            var imageTypes = AcceptableContentTypes("image") ?? Array.Empty<string>();
            var videoTypes = AcceptableContentTypes("video") ?? Array.Empty<string>();

            var videoVM = new VideoVM
            {
                CategoryList = await GetCategoryListAsync(),
                ImageContentTypes = imageTypes.Any() ? string.Join(",", imageTypes) : string.Empty,
                VideoContentTypes = videoTypes.Any() ? string.Join(",", videoTypes) : string.Empty,
            };

            if (id.HasValue && id.Value > 0)
            {
                var video = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                    x => x.Id == id.Value,
                    includeProperties: "Category,Channel");

                if (video != null)
                {
                    
                    videoVM.Id = video.Id;
                    videoVM.Title = video.Title;
                    videoVM.Description = video.Description;
                    videoVM.CategoryId = video.CategoryId;
                    videoVM.ImageUrl = video.Thumbnail;
                  
                }
            }

            return videoVM;
        }

        public async Task<bool> UserHasChannelAsync()
        {
           var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var channel=await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x => x.AppUserId == userId);
            if (channel != null)
            {
                return true;
            }
            return false;
        }

        public Task<IEnumerable<VideoVM>> GetVideosByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VideoVM>> GetVideosByChannelIdAsync(int channelId)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorModel<Videos>> UpdateVideoAsync(VideoVM model)
        {
            throw new NotImplementedException();
        }

        #region Private Method

        private Func<IQueryable<Videos>, IOrderedQueryable<Videos>> GetOrderByExpression(string sortBy)
        {
            return sortBy switch
            {
                "title-a" => q => q.OrderBy(x => x.Title),
                "title-d" => q => q.OrderByDescending(x => x.Title),
                "date-a" => q => q.OrderBy(x => x.CreatedAt),
                "date-d" => q => q.OrderByDescending(x => x.CreatedAt),
                "Category-a" => q => q.OrderBy(x => x.Category.Name),
                "Category-d" => q => q.OrderByDescending(x => x.Category.Name),
                _ => q => q.OrderByDescending(x => x.CreatedAt)
            };
        }
        private async Task<byte[]> ProcessUploadedFiles(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            contents = memoryStream.ToArray();
            return contents;

        }
        private async Task<IEnumerable<SelectListItem>> GetCategoryListAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

        private string[] AcceptableContentTypes(string type)
        {
            if (type.Equals("image"))
            {
                return _configuration.GetSection("FileUpload:ImageContentTypes").Get<string[]>();
            }
            else
            {
                return _configuration.GetSection("FileUpload:VideoContentTypes").Get<string[]>();
            }
        }

        private bool IsAcceptableContentType(string type, string contentType)
        {
            var allowedContentTypes = AcceptableContentTypes(type);
            foreach (var allowedContentType in allowedContentTypes)
            {
                if (contentType.ToLower().Equals(allowedContentType.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<ErrorModel<Videos>> CreateNewVideos(
           VideoVM model, int channelId, byte[] thumbnailBytes, byte[] videoBytes)
        {
            var newVideo = new Videos
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ChannelId = channelId,
                Thumbnail = _picService.UploadPics(model.ImageUpload),
                ContentType = model.VideoUpload.ContentType,
                Contents = ProcessUploadedFiles(model.VideoUpload).GetAwaiter().GetResult(),
            };

            _unitOfWork.VideosRepository.Add(newVideo);
            await _unitOfWork.CompleteAsync();
            return ErrorModel<Videos>.Success(newVideo, "Video created successfully");
        }

        private async Task<ErrorModel<Videos>> UpdateExistingVideo(
            VideoVM model, byte[] thumbnailBytes, byte[] videoBytes)
        {
            var existingVideo = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(x => x.Id == model.Id);
            if (existingVideo == null)
                return ErrorModel<Videos>.Failure("Video not found", 404);
            existingVideo.Id=model.Id;
            existingVideo.Title = model.Title;
            existingVideo.Description = model.Description;
            existingVideo.CategoryId = model.CategoryId;

            if (model.ImageUpload != null)
            {
                existingVideo.Thumbnail = _picService.UploadPics(model.ImageUpload);
            }
            else
            {
                existingVideo.Thumbnail = model.ImageUrl;
            }
            if (model.VideoUpload != null)
            {
                existingVideo.ContentType = model.VideoUpload.ContentType;
                existingVideo.Contents = videoBytes;
            }


            _unitOfWork.VideosRepository.UpdateAsync(existingVideo);
            await _unitOfWork.CompleteAsync();
            return ErrorModel<Videos>.Success(existingVideo, "Video updated successfully");
        }

       






        #endregion
    }

}
