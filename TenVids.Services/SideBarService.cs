using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Models.DTOs;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;

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
