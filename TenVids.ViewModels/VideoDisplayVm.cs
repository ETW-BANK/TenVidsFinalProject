
namespace TenVids.ViewModels
{
    public class VideoDisplayVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
       public string CategoryName { get; set; } 
        public string Thumbnailurl { get; set; } 
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public DateTime CreatedAt { get; set; }
  
    }
}
