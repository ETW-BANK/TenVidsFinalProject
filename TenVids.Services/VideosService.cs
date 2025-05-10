using Microsoft.AspNetCore.Http;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;
using Microsoft.Extensions.Options;
using TenVids.Models.Pagination;
using TenVids.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TenVids.Services.HelperMethods;
using TenVids.Utilities.FileHelpers;
using AutoMapper;

namespace TenVids.Services
{
    public class VideosService: IVideosService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly FileUploadConfig _fileUploadConfig;
        private readonly IVideoViewService _videoViewService;
        private readonly IHelper _helper;
        private readonly IPicService _picservice;
        private readonly IMapper _mapper;
        
        public VideosService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor,  IOptions<FileUploadConfig> fileUploadConfig,IHelper helper,IVideoViewService videoViewService,IPicService picservice,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _fileUploadConfig = fileUploadConfig.Value;
            _helper = helper;
            _videoViewService = videoViewService;
            _picservice= picservice;
            _mapper = mapper;

        }
        public async Task<PaginatedResult<VideoForHomeDto>> GetVideosForHomeGridAsync(HomeParameters parameters)
        {
            var userid = _httpContextAccessor?.HttpContext?.User.GetUserId();   
         
            var paginatedList = await _helper.GetVideos(parameters);

            return new PaginatedResult<VideoForHomeDto>(
                items: paginatedList.ToList(), 
                totalCount: paginatedList.TotalCount,
                pageSize: parameters.PageSize,
                pageNumber: parameters.PageNumber,
                totalPages: (int)Math.Ceiling(paginatedList.TotalCount / (double)parameters.PageSize)
            );
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

            if (!_helper.IsAcceptableContentType("image", model.ImageUpload.ContentType))
            {
                var allowedTypes = _helper.AcceptableContentTypes("image");
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

            if (!_helper.IsAcceptableContentType("video", model.VideoUpload.ContentType))
            {
                var allowedTypes = _helper.AcceptableContentTypes("video");
                return ErrorModel<Videos>.Failure(
                    $"Invalid video type. Allowed: {string.Join(", ", allowedTypes)}",
                    400);
            }

            if (model.VideoUpload.Length > _fileUploadConfig.VideoMaxSizeInMB * SD.fileSizeLimit)
                return ErrorModel<Videos>.Failure(
                    $"Video exceeds maximum size of {_fileUploadConfig.VideoMaxSizeInMB}MB",
                    400);

            // Process uploaded files
            var thumbnailBytes = await _helper.ProcessUploadedFiles(model.ImageUpload);
            var videoBytes = await _helper.ProcessUploadedFiles(model?.VideoUpload);


            if (model.Id == 0)
            {
                return await _helper.CreateNewVideos(model, userChannel.Id, thumbnailBytes, videoBytes);
            }
            else
            {
                return await _helper.UpdateExistingVideo(model, thumbnailBytes, videoBytes);
            }
        }

        public async Task<PaginatedResult<VideoGridChannelDto>> GetVideosForChannelAsync(BaseParams parameters)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

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
                            Dislikes = SD.GetRandomNumber(5, 100, video.Id),
                            SubscribersCount = channel.Subscribers.Count()
                           
                        };

            query = parameters.SortBy switch
            {
                "title-a" => query.OrderBy(x => x.Title),
                "title-d" => query.OrderByDescending(x => x.Title),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            // Single roundtrip for count 
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
                var userId = _httpContextAccessor?.HttpContext?.User.GetUserId();

                var existingVideo = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                    x => x.Id == id,
                    includeProperties: "Comments,VideoViewers,VideoFile,Likes,Channel,Channel.AppUser",
                    tracked: true);

                if (existingVideo == null)
                {
                    return ErrorModel<Videos>.Failure("Video not found", 404);
                }

               
                if (existingVideo.Channel?.AppUser?.Id != userId)
                {
                    return ErrorModel<Videos>.Failure("You don't have permission to delete this video", 403);
                }

              
                if (existingVideo.Comments?.Any() == true)
                {
                    _unitOfWork.CommentsRepository.RemoveRange(existingVideo.Comments);
                }

                if (existingVideo.Likes?.Any() == true)
                {
                _unitOfWork.LikesRepository.RemoveRange(existingVideo.Likes);   
                }

                if (existingVideo.VideoViewers?.Any() == true)
                {
                    _unitOfWork.VideoViewRepository.RemoveRange(existingVideo.VideoViewers);
                }

                if (existingVideo.VideoFile != null)
                {
                    _unitOfWork.VideoFileRepository.Remove(existingVideo.VideoFile);
                }

               
                _picservice.DeletePhotoLocally(existingVideo.Thumbnail);

               
                _unitOfWork.VideosRepository.Remove(existingVideo);

                await _unitOfWork.CompleteAsync();
                return ErrorModel<Videos>.Success(existingVideo, $"Video '{existingVideo.Title}' deleted successfully");
            }
            catch (Exception ex)
            {
               
                return ErrorModel<Videos>.Failure($"An error occurred while deleting the video: {ex.Message}", 500);
            }
        }
        public async Task<IEnumerable<VideoVM>> GetAllVideosAsync()
        {
            var result = await _unitOfWork.VideosRepository.GetAllAsync(
                               includeProperties: "Category,Channel",
                                              orderby:_helper.GetOrderByExpression("date-d"));

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

            var imageTypes = _helper.AcceptableContentTypes("image") ?? Array.Empty<string>();
            var videoTypes =_helper.AcceptableContentTypes("video") ?? Array.Empty<string>();

            var videoVM = new VideoVM
            {
                CategoryList = await _helper.GetCategoryListAsync(),
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
        public async Task<WatchVideoVM?> GetVideoToWatchAsync(int videoId)
        {
            var video = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                x => x.Id == videoId,
                includeProperties: "Channel.Subscribers,Likes,Comments.AppUser,VideoViewers");

            if (video == null)
            {
                return null; 
            }

            var suggestedVideos = _unitOfWork.VideosRepository.GetQueryable()
                .Where(x => x.ChannelId == video.ChannelId && x.Id != video.Id)
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .ToList();

            var userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                userId = "Anonymous"; 
            }

            var clientIpAddress = await _helper.GetClientIpAddressAsync() ?? "127.0.1.1";

            var availableComments = video.Comments?.Where(c => c != null).Select(c => new AvailableCommentsVM
            {
                Content = c.Content,
                FormName = c.AppUser?.UserName ?? "Unknown",
                FormChannelId = c.AppUser?.Channel?.Id ?? 0,
                PostedAt = c.PostedAt
            }).ToList();

            var result = new WatchVideoVM
            {
                Id = video.Id,
                Title = video.Title ?? string.Empty,
                Description = video.Description ?? string.Empty,
                CreatedAt = video.CreatedAt,
                ChannelId = video.ChannelId,
                ChannelName = video.Channel?.Name ?? string.Empty,

                IsSubscribed = video.Channel?.Subscribers?.Any(x => x.AppUserId == userId) ?? false, 
                IsLiked = video.Likes?.Any(x => x.AppUserId == userId && x.IsLike == true) ?? false, 
                IsDisliked = video.Likes?.Any(x => x.AppUserId == userId && x.IsLike == false) ?? false,

                SubscribersCount = video.Channel?.Subscribers?.Count() ?? 0,
                ViewsCount = video.VideoViewers?.Select(x => x.NumberOfVisits).Sum() ?? 0, 
                LikesCount = video.Likes?.Count(x => x.IsLike == true) ?? 0,
                DislikesCount = video.Likes?.Count(x => x.IsLike == false) ?? 0,

                CommentVM = new CommentsVM
                {
                    PostComment = new CommentPostVM
                    {
                        VideoId = video.Id,
                        Content = string.Empty
                    },
                    AvailableComments = video.Comments?.Where(c => c != null).Select(c => new AvailableCommentsVM
                    {
                        Content = c.Content,
                        FormName = c.AppUser?.UserName ?? "Unknown",
                        FormChannelId = _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x => x.AppUserId == c.AppUserId).Result?.Id ?? 0,
                        PostedAt = c.PostedAt
                    }).ToList()
                },
                SuggestedVideos = suggestedVideos
            };

            await _videoViewService.HandleVideoViewAsync(userId, videoId, clientIpAddress); 
            await _unitOfWork.CompleteAsync();
            return result;
        }



        public async Task<ErrorModel<string>> LikeVideo(int videoId, string action, bool like)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return ErrorModel<string>.Failure("User not authenticated", 401);
                }

                var video = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(
                    x => x.Id == videoId,
                    includeProperties: "Likes",
                    tracked: true); 

                if (video == null)
                {
                    return ErrorModel<string>.Failure("Video not found", 404);
                }

                // Initialize collection if null
                video.Likes ??= new List<Likes>();

                var existingLike = video.Likes.FirstOrDefault(x => x.AppUserId == userId);

                string command = "";

                if (action == "like")
                {
                    if (existingLike == null)
                    {
                        // Add new like
                        var newLike = new Likes
                        {
                            AppUserId = userId,
                            VideoId = videoId,
                            IsLike = true
                        };
                        video.Likes.Add(newLike);
                        command = "addLike";
                    }
                    else if (!existingLike.IsLike)
                    {
                        // Change dislike to like
                        existingLike.IsLike = true;
                        command = "removeDislike-addLike";
                    }
                    else
                    {
                        // Remove existing like
                        video.Likes.Remove(existingLike);
                        command = "removeLike";
                    }
                }
                else if (action == "dislike")
                {
                    if (existingLike == null)
                    {
                        // Add new dislike is liked false
                        var newDislike = new Likes
                        {
                            AppUserId = userId,
                            VideoId = videoId,
                            IsLike = false
                        };
                        video.Likes.Add(newDislike);
                        command = "addDislike";
                    }
                    else if (existingLike.IsLike)
                    {
                        // Change like to dislike
                        existingLike.IsLike = false;
                        command = "removeLike-addDislike";
                    }
                    else
                    {
                        // Remove existing dislike
                        video.Likes.Remove(existingLike);
                        command = "removeDislike";
                    }
                }
                else
                {
                    return ErrorModel<string>.Failure("Invalid action", 400);
                }

             
                _unitOfWork.VideosRepository.Update(video);

               
                bool changesSaved = await _unitOfWork.CompleteAsync();

                if (changesSaved)
                {
                    
                    return ErrorModel<string>.Success(command);
                }
                else
                {
                    return ErrorModel<string>.Failure("No changes were saved", 500);
                }
            }
            catch (Exception ex)
            {
              
                return ErrorModel<string>.Failure("Server Error", 500);
            }
        }

        public async Task<ErrorModel<VideoFileDto>> DownloadVideoFileAsync(int id)
        {
            try
            {
              
                var videoFile = await _unitOfWork.VideoFileRepository.GetQueryable()
                    .Include(vf => vf.Video) 
                    .FirstOrDefaultAsync(vf => vf.VideoId == id);

                if (videoFile == null)
                {
                    return ErrorModel<VideoFileDto>.Failure("Video file not found", 404);
                }

                if (videoFile.Video == null)
                {
                    return ErrorModel<VideoFileDto>.Failure("Associated video not found", 404);
                }

                return ErrorModel<VideoFileDto>.Success(
                    new VideoFileDto
                    {
                        Contents = videoFile.Contents,
                        ContentType = videoFile.ContentType,
                        FileName = $"{videoFile.Video.Title}{videoFile.Extension}"
                    },
                    "Video file retrieved successfully");
            }
            catch (Exception ex)
            {
                
                return ErrorModel<VideoFileDto>.Failure("Error retrieving video file", 500);
            }
        }
        public async Task<VideoFileDto?> GetVideoFileAsync(int id)
        {
            if (id <= 0) return null;

            return await _unitOfWork.VideosRepository.GetQueryable()
                .Where(v => v.Id == id)
                .Select(v => new VideoFileDto
                {
                    Contents = v.VideoFile.Contents,
                    ContentType = v.VideoFile.ContentType,
                  
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public async Task<List<object>> GetVideosByChannelIdAsync(int channelId)
        {
            var videos = await _unitOfWork.VideosRepository.GetQueryable()
                .Where(x => x.ChannelId == channelId)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.ChannelId,
                    ThumbnailUrl = x.Thumbnail.Replace("\\", "/"),
                    x.Description,
                    CreatedAtTimeAgo = SD.TimeAgo(x.CreatedAt),
                    x.CreatedAt,
                    NumberOfViews = x.VideoViewers.Count()
                })
                .ToListAsync();

            return videos.Cast<object>().ToList();
        }

        public async Task<ErrorModel<IEnumerable<VideoDisplayVm>>> AllVideos()
        {
            var result= await _unitOfWork.VideosRepository.GetAllAsync(
                               includeProperties: "Category,Channel",
                                              orderby: _helper.GetOrderByExpression("date-d"));
            var vidios=_mapper.Map<IEnumerable<VideoDisplayVm>>(result);
            if (vidios != null)
            {
                return ErrorModel<IEnumerable<VideoDisplayVm>>.Success(vidios.ToList(), "Videos retrieved successfully");
            }
            else
            {
                return ErrorModel<IEnumerable<VideoDisplayVm>>.Failure("No videos found", 404);
            }
        }

        public async Task<ErrorModel<VideoDisplayVm>> DeleteVideos(int id)
        {
            var video = await _unitOfWork.VideosRepository.GetQueryable()
                .Where(x => x.Id == id)
                .Select(x => new { x.Id, x.Title, x.Thumbnail })
                .FirstOrDefaultAsync();

            if (video == null)
            {
                return ErrorModel<VideoDisplayVm>.Failure("Video not found", 404);
            }

       
            _picservice.DeletePhotoLocally(video.Thumbnail);

         
            var entity = await _unitOfWork.VideosRepository.GetByIdAsync(video.Id);
            if (entity != null)
            {
                _unitOfWork.VideosRepository.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }

            return ErrorModel<VideoDisplayVm>.Success(new VideoDisplayVm
            {
                Id = video.Id,
                Title = video.Title,
                Thumbnailurl = video.Thumbnail
            }, "Video deleted successfully");
        }

    }

}
