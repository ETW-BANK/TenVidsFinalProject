using TenVids.Data.Access.Data;
using TenVids.Data.Access.IRepo;
using TenVids.Models;

namespace TenVids.Repository
{
    public class ChannelRepository : Repository<Channel>, IChannelRepository
    {
        public ChannelRepository(TenVidsApplicationContext context) : base(context)
        {
            
        }

    }  
  
 }

