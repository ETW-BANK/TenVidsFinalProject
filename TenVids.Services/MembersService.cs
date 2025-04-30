



using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class MembersService : IMembersService
    {

        private readonly TenVidsApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MembersService(TenVidsApplicationContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<MemberVM> GetmembersChannel(int id)
        {
            var userid = _httpContextAccessor.HttpContext.User.GetUserId();
            var fetchedChannel = await _context.Channels
                 .Where(c => c.Id == id)
                 .Select(x => new MemberVM
                 {
                     ChannelId = x.Id,
                     Name = x.Name,
                     About = x.Description,
                     CreatedAt = x.CreatedAt,
                     NumberOfAvailableVideos = x.Videos.Count(),
                     NumberOfSubscribers = x.Subscribers.Count(),
                     UserIsSubscribed = x.Subscribers.Any(x => x.AppUserId == userid)
                 }).FirstOrDefaultAsync();

            return fetchedChannel;
        }
    }
}
