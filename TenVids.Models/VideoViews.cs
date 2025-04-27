
namespace TenVids.Models
{
    public class VideoViews:BaseEntity
    {
        public string IpAddress { get; set; }
        public int NumberOfVisits { get; set; } = 1;
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }

        public bool Is_Proxy { get; set; }  
        public DateTime LastVisit { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set; } 
        public int VideoId { get; set; }   
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }   
    }
}
