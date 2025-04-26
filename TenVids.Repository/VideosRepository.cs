using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class VideosRepository : Repository<Videos>, IVideosRepository
    {
        public VideosRepository(TenVidsApplicationContext context) : base(context)
        {

        }

    }
  }