using System.ComponentModel.DataAnnotations;
using Tensae.Generic.Repository;

namespace TenVids.Models
{
    public class Channel:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set;} 
        public ApplicationUser AppUser { get; set; }
        public ICollection<Videos>? Videos { get; set; } 
        public ICollection<Subscribe> Subscribers { get; set; }

    }
}
