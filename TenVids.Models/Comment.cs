using System.ComponentModel.DataAnnotations;

namespace TenVids.Models
{
    public class Comment:BaseEntity
    {
        public Comment()
        {
            
        }
        public Comment(string appUserId, int videoId, string content)
        {
            AppUserId = appUserId;
            VideoId = videoId;
            Content = content;
        }

        public string AppUserId { get; set; }
        public int VideoId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }
    }
}
