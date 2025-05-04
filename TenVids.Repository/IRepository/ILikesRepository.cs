

using TenVids.Models;

namespace TenVids.Repository.IRepository
{
   public interface ILikesRepository
    {
        void RemoveRange(IEnumerable<Likes> likes);
    }
}
