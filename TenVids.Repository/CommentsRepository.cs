using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class CommentsRepository:Repository<Comment>, ICommentsRepository
    {
        public CommentsRepository(TenVidsApplicationContext context) : base(context)
        {
        }
    }
   
}
