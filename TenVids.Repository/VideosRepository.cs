using TenVids.Data.Access.Data;
using TenVids.Data.Access.IRepo;
using Tensae.Generic.Repository;
using TenVids.Models;

namespace TenVids.Repository
{
    public class  VideosRepository : Repository<Videos>, IVideosRepository
    {
        public VideosRepository(TenVidsApplicationContext context) : base(context)
        {

        }

    }
  }