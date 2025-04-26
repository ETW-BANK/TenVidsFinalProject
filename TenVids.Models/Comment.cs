using System.ComponentModel.DataAnnotations;

namespace TenVids.Models
{
    public class Comment
    {
        public int VideoId { get; set; }
        public string AppUserId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }
    }
}
