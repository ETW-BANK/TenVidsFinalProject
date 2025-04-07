using AutoMapper;
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


namespace TenVids.Services
{
    public class VideosService: IVideosService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly FileUploadConfig _fileUploadConfig;
        private readonly IPicService _picService;   

        public VideosService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor, IMapper mapper,IConfiguration configuration, IOptions<FileUploadConfig> fileUploadConfig,IPicService picService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _picService = picService;
            _mapper = mapper;
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
            var videoBytes = await ProcessUploadedFiles(model.VideoUpload);

            
            if (model.Id == 0)
            {
                return await CreateNewVideos(model, userChannel.Id, thumbnailBytes, videoBytes);
            }
            else
            {
                return await UpdateExistingVideo(model, thumbnailBytes, videoBytes);
            }
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
        public Task DeleteVideoAsync(Videos video)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VideoVM>> GetAllVideosAsync()
        {
            throw new NotImplementedException();
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

        #region Private Method
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
