using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;

namespace TenVids.Repository
{
    public class VideosRepository : Repository<Videos>, IVideosRepository
    {
        public VideosRepository(TenVidsApplicationContext context) : base(context)
        {

        }

    }
  }