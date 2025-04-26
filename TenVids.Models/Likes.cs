
namespace TenVids.Models
{
    public class Likes
    {
        public Likes()
        {
            
        }
        public Likes(string appUserId, int videoId)
        {
            AppUserId = appUserId;
            VideoId = videoId;
        }

        public bool IsLike { get; set; } 
        public string AppUserId { get; set; }
        public int VideoId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }
    }
}
