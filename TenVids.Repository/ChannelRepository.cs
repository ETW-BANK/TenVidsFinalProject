
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class ChannelRepository:Repository<Channel>, IChannelRepository
    {
        private  readonly TenVidsApplicationContext _context;

        public ChannelRepository(TenVidsApplicationContext context):base(context)
        {
            _context = context;
        }
        public async Task<Channel?> GetByUserIdAsync(string userId, string? includeProperties = null)
        {
            return await GetFirstOrDefaultAsync(
                x => x.AppUserId == userId,
                includeProperties,
                tracked: false);
        }
        public async Task CreateAsync(Channel channel)
        {
            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UserHasChannelAsync(string userId)
        {
            return await _context.Channels
                .AnyAsync(c => c.AppUserId == userId);
        }
    }
 }

