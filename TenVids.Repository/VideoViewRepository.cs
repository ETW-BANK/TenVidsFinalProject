using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
   public class VideoViewRepository: Repository<VideoViews>, IVideoViewRepository
    {
        public VideoViewRepository(TenVidsApplicationContext context) : base(context)
        {
        }
    }
   
}
