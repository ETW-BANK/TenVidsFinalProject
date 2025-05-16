using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;

namespace TenVids.Repository
{
   public class VideoFileRepository:Repository<VideoFiles>, IVideoFileRepository
    {
        public VideoFileRepository(TenVidsApplicationContext context):base(context)
        {

        }
   
    }
}
