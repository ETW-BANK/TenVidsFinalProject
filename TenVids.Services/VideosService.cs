using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.FileManupliation.Helpers;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class VideosService: IVideosService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IFileTypeHelper _fileTypeHelper;
        private readonly IMapper _mapper;

        public VideosService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor, IFileTypeHelper fileTypeHelper,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _fileTypeHelper = fileTypeHelper;
            _mapper = mapper;
        }

        public async Task<ErrorModel<Videos>> CreateEditVideoAsync(VideoVM model)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userChannel = await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x => x.AppUserId == userId);

            if (userChannel == null)
            {
                return ErrorModel<Videos>.Failure("No Channel Found  With Your Account", 404);
            }
            if (model.Id == 0)
            {
                var videoExists= await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(x => x.Title == model.Title && x.ChannelId == userChannel.Id);

                if(videoExists!=null)
                {
                    return ErrorModel<Videos>.Failure("Video with this title already exists", 409);
                }
                var video=model.Id == 0 ? new Videos() : await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(x => x.Id == model.Id);

                _mapper.Map(model, video);
                video.ChannelId = userChannel.Id;

                if(model.Id == 0)
                {
                    _unitOfWork.VideosRepository.Add(video);
                }
                else
                {
                    _unitOfWork.VideosRepository.UpdateAsync(video);
                }
                await _unitOfWork.CompleteAsync();
            }
            return ErrorModel<Videos>.Success(model.Id == 0 ? "Video created" : "Video updated");

        }
        public async Task<VideoVM> GetVideoByIdAsync(int? id)
        {
            if (!await UserHasChannelAsync())
            {
                return null;
            }

            var imageTypes = _fileTypeHelper.AcceptableContentTypes("image") ?? Array.Empty<string>();
            var videoTypes = _fileTypeHelper.AcceptableContentTypes("video") ?? Array.Empty<string>();

            var videoVM = new VideoVM
            {
                CategoryList = await GetCategoryListAsync(),
                ImageContentTypes = imageTypes.Any() ? string.Join(",", imageTypes) : string.Empty,
                VideoContentTypes = videoTypes.Any() ? string.Join(",", videoTypes) : string.Empty,
            };

            if (id.HasValue && id.Value > 0)
            {
                var video = await _unitOfWork.VideosRepository.GetFirstOrDefaultAsync(x => x.Id == id.Value);
                if (video != null)
                {
                    _mapper.Map(video, videoVM);
                }
            }

            return videoVM;
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

        private async Task<IEnumerable<SelectListItem>> GetCategoryListAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

        

        #endregion
    }

}
