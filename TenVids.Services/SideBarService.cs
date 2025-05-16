using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Models.DTOs;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.Utilities;

namespace TenVids.Services
{
    public class SideBarService:ISideBarService
    {

        private readonly TenVidsApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SideBarService(TenVidsApplicationContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<HistoryDto>> GetHistory()
        {
            var userId=_httpContextAccessor.HttpContext.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Enumerable.Empty<HistoryDto>();
            }

            return await _context.VideoViews
                .Where(x=>x.AppUserId == userId)
                .Select(x=>new HistoryDto
                {
                    Id=x.VideoId,
                    Title=x.Video.Title,
                    ChannelName=x.Video.Channel.Name,
                    ChannelId=x.Video.Channel.Id,
                    LastVisitTimeAgo=SD.TimeAgo(x.LastVisit),
                    LastVisit=x.LastVisit,
                }).ToListAsync();
        }

        public async Task<IEnumerable<LikeDislikeDto>> GetLikeDislike(bool liked)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Enumerable.Empty<LikeDislikeDto>();
            }

            var result = await _context.Likes
                .Where(x => x.AppUserId == userId && x.IsLike == liked)  
                .Select(x => new LikeDislikeDto
                {
                    Id = x.VideoId,
                    Title = x.Video.Title,
                    Thumbnail = x.Video.Thumbnail,
                    ChannelName = x.Video.Channel.Name,
                    ChannelId = x.Video.Channel.Id,
                    CreatedAtTimeAgo = SD.TimeAgo(x.Video.CreatedAt),
                    CreatedAt = x.Video.CreatedAt,
                    IsLiked = liked
                })
                
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetSubscriptions()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Enumerable.Empty<SubscriptionDto>();

            return await _context.Subscribe
                .Where(x => x.AppUserId == userId)
                .Select(x => new SubscriptionDto
                {
                    Id = x.ChannelId,
                    ChannelName = x.Channel.Name,
                    VideosCount = x.Channel.Videos.Count
                })
                .ToListAsync();
        }
    }
}
