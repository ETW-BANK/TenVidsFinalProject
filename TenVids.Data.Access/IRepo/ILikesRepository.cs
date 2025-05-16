using TenVids.Models;

namespace TenVids.Data.Access.IRepo
{
   public interface ILikesRepository
    {
        void RemoveRange(IEnumerable<Likes> likes);
    }
}
