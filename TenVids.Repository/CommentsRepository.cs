using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;

namespace TenVids.Repository
{
    public class CommentsRepository:Repository<Comment>, ICommentsRepository
    {
        public CommentsRepository(TenVidsApplicationContext context) : base(context)
        {
        }
    }
   
}
