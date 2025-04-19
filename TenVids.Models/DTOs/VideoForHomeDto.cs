
namespace TenVids.Models.DTOs
{
   public class VideoForHomeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public string ChannelName { get; set; }
        public int ChannelId { get; set; }
        public int CategoryId { get; set; }
     
    }
}
