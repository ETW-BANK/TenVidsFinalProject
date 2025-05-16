using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Data.Access.IRepo;

namespace TenVids.Repository
{
 public class LikesRepository: ILikesRepository
    {
        private readonly TenVidsApplicationContext _context;

        public LikesRepository(TenVidsApplicationContext context)
        {
            _context = context;
        }

        public void RemoveRange(IEnumerable<Likes> likes)
        {
            _context.Likes.RemoveRange(likes);
        }
    }
   
    }

