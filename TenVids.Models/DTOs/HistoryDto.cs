
namespace TenVids.Models.DTOs
{
   public class HistoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ChannelName { get; set; } 
        public int ChannelId { get; set; }
        public string LastVisitTimeAgo { get; set; } 
        public DateTime LastVisit { get; set;}
    }
}
