
namespace TenVids.Models
{
   public class Subscribe
    {
        public Subscribe()
        {
            
        }
        public Subscribe(string appUserId, int channelId)
        {
            AppUserId = appUserId;
            ChannelId = channelId;
        }
        public string AppUserId { get; set; } 
        public int ChannelId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public Channel Channel { get; set; }
    }
}
