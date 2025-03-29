
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
   public class VideosRepository:Repository<Videos>, IVideosRepository
    {
        private readonly TenVidsApplicationContext _context;

        public VideosRepository(TenVidsApplicationContext context) : base(context)
        {
            _context = context;
        }
    }
}
