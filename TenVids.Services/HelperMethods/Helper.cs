using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenVids.Models.DTOs;
using TenVids.Models.Pagination;
using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;
using TenVids.Repository.IRepository;
using TenVids.Utilities.FileHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Net;

namespace TenVids.Services.HelperMethods
{
   public class Helper:IHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPicService _picService;
        private readonly HttpClient _httpClient;

        public Helper(IUnitOfWork unitOfWork,IConfiguration configuration, IPicService picService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
          
            _configuration = configuration;
          
            _picService = picService;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.ip2location.io")
            };
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginatedList<VideoForHomeDto>> GetVideos(HomeParameters parameters)
        {


            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parameters.PageNumber = Math.Max(1, parameters.PageNumber);
            parameters.PageSize = Math.Clamp(parameters.PageSize, 1, 100);

            try
            {

                var query = _unitOfWork.VideosRepository.GetQueryable()
                 .Include(v => v.Category)
                .Include(v => v.Channel)
                 .AsQueryable();

                if (parameters.CategoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == parameters.CategoryId);
                }

                if (!string.IsNullOrWhiteSpace(parameters.SearchBy))
                {
                    var searchTerm = parameters.SearchBy.Trim().ToLower();
                    query = query.Where(x =>
                        x.Title.ToLower().Contains(searchTerm) ||
                        (x.Description != null && x.Description.ToLower().Contains(searchTerm))
                    );
                }

                query = parameters.SortBy?.ToLower() switch
                {

                    "newest" => query.OrderByDescending(x => x.CreatedAt),
                    "oldest" => query.OrderBy(x => x.CreatedAt),
                    _ => query.OrderByDescending(x => x.CreatedAt)
                };

                var resultQuery = query.Select(x => new VideoForHomeDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ThumbnailUrl = x.Thumbnail,
                    CreatedAt = x.CreatedAt,
                    Views = SD.GetRandomNumber(10000, 500000, x.Id),
                    ChannelName = x.Channel.Name,
                    ChannelId = x.ChannelId,
                    CategoryId = x.CategoryId,

                });

                return await PaginatedList<VideoForHomeDto>.CreateAsync(
                    resultQuery.AsNoTracking(),
                    parameters.PageNumber,
                    parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving videos", ex);
            }
        }

       public Func<IQueryable<Videos>, IOrderedQueryable<Videos>> GetOrderByExpression(string sortBy)
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
       public async Task<byte[]> ProcessUploadedFiles(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            contents = memoryStream.ToArray();
            return contents;

        }
       public async Task<IEnumerable<SelectListItem>> GetCategoryListAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

       public string[] AcceptableContentTypes(string type)
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

       public bool IsAcceptableContentType(string type, string contentType)
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
        public async Task<ErrorModel<Videos>> CreateNewVideos(
             VideoVM model, int channelId, byte[] thumbnailBytes, byte[] videoBytes)
        {
            var newVideo = new Videos
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ChannelId = channelId,
                Thumbnail = _picService.UploadPics(model.ImageUpload),
                VideoFile = new VideoFiles
                {
                    ContentType = model.VideoUpload.ContentType,
                    Contents = ProcessUploadedFiles(model.VideoUpload).GetAwaiter().GetResult(),
                    Extension = SD.GetExtension(model.VideoUpload.ContentType),
                },
            };

            _unitOfWork.VideosRepository.Add(newVideo);
            await _unitOfWork.CompleteAsync();
            return ErrorModel<Videos>.Success(newVideo, "Video created successfully");
        }

        public async Task<ErrorModel<Videos>> UpdateExistingVideo(
         VideoVM model, byte[] thumbnailBytes, byte[] videoBytes)
        {
            var existingVideo = await _unitOfWork.VideosRepository
                .GetFirstOrDefaultAsync(
                    x => x.Id == model.Id,
                    includeProperties: "VideoFile"); 

            if (existingVideo == null)
                return ErrorModel<Videos>.Failure("Video not found", 404);

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
             
                if (existingVideo.VideoFile == null)
                {
                    existingVideo.VideoFile = new VideoFiles(); 
                }

                existingVideo.VideoFile.ContentType = model.VideoUpload.ContentType;
                existingVideo.VideoFile.Contents = videoBytes;
            }

            _unitOfWork.VideosRepository.UpdateAsync(existingVideo);
            await _unitOfWork.CompleteAsync();

            return ErrorModel<Videos>.Success(existingVideo, "Video updated successfully");
        }


        public async Task<VideoViews> AddVideoViewAsync(string userId, int videoId, string ipAddress)
        {
            ipAddress = SD.NormalizeIp(ipAddress);
            var ip2LocationResult = await GetIP2LocationResultAsync(ipAddress);

            var videoViewToAdd = new VideoViews
            {
                AppUserId = userId,
                VideoId = videoId,
                IpAddress = ipAddress,
                Country = ip2LocationResult.Country_Name,
                City = ip2LocationResult.City_Name,
                PostCode = ip2LocationResult.Zip_Code,
                Is_Proxy = ip2LocationResult.Is_Proxy,
                LastVisit = DateTime.UtcNow,
                NumberOfVisits = 1
            };

            _unitOfWork.VideoViewRepository.Add(videoViewToAdd);
            await _unitOfWork.CompleteAsync();

            return videoViewToAdd;
        }

        public async Task<IP2LocationResultDto> GetIP2LocationResultAsync(string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress) ||
                    IPAddress.IsLoopback(IPAddress.Parse(ipAddress)) ||
                    ipAddress.StartsWith("192.168.") ||
                    ipAddress.StartsWith("10.") ||
                    ipAddress.StartsWith("172."))
                {
                    return new IP2LocationResultDto
                    {
                        Country_Name = "Local",
                        City_Name = "Development"
                    };
                }

                var response = await _httpClient.GetFromJsonAsync<IP2LocationResultDto>(
                    $"?key={_configuration["IP2LocationAPIKey"]}&ip={ipAddress}&format=json");

                return response ?? new IP2LocationResultDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IP2Location error for {ipAddress}: {ex.Message}");
                return new IP2LocationResultDto();
            }
        }

        public async Task<string> GetClientIpAddressAsync()
        {
            try
            {
                var httpContext =_httpContextAccessor?.HttpContext;
                if (httpContext == null)
                    return "127.0.0.1"; 

             
                if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    var ip = forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim();
                    if (!string.IsNullOrEmpty(ip))
                        return SD.NormalizeIp(ip);
                }

                var remoteIp = httpContext.Connection?.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(remoteIp))
                    return SD.NormalizeIp(remoteIp);

                return "127.0.0.1"; 
            }
            catch
            {
                return "127.0.0.1"; 
            }
        }


    }
}
