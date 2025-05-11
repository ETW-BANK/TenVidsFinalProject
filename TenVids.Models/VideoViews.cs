
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenVids.Models
{
    public class VideoViews:BaseEntity
    {
        public string AppUserId { get; set; }
        public int VideoId { get; set; }


        // IP2Location
        public string IpAddress { get; set; }

        [Column("NumberOfVisits")]
        public int NumberOfVisit { get; set; } = 1;
        public string City { get; set; }
        [Column("PostCode")]
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool Is_Proxy { get; set; }
        public DateTime LastVisit { get; set; } = DateTime.UtcNow;


        // Navigations
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }
    }
}
