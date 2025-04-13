
namespace TenVids.Models.DTOs
{
    public class VideoGridChannelDto
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string CategoryName { get; set; }
    }
}
