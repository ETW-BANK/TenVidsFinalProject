

namespace TenVids.Models.DTOs
{
    public class LikeDislikeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string ChannelName { get; set; }
        public int ChannelId { get; set; }
        public string CreatedAtTimeAgo { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLiked { get; set; }
    }
}
