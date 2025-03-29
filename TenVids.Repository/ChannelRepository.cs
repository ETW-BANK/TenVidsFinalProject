using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
