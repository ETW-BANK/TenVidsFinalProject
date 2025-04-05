
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class ChannelRepository : Repository<Channel>, IChannelRepository
    {
       
        public ChannelRepository(TenVidsApplicationContext context) : base(context)
        {
            
        }

      
    }  

       
 }

