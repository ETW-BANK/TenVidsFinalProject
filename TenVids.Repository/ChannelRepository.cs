

using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class ChannelRepository : Repository<Channel>, IChannelRepository
    {
        private readonly TenVidsApplicationContext _context;

        public ChannelRepository(TenVidsApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> UserHasChannelAsync(string userId)
        {
            return await _context.Channels
                .AnyAsync(c => c.AppUserId == userId);
        }
    }  

       
 }

