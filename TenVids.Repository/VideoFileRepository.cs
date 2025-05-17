using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;
using Tensae.Generic.Repository;
namespace TenVids.Repository
{
   public class VideoFileRepository:Repository<VideoFiles>, IVideoFileRepository
    {
        public VideoFileRepository(TenVidsApplicationContext context):base(context)
        {

        }
   
    }
}
