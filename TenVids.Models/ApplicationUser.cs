using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TenVids.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Channel? Channel { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Subscribe> Subscribtions { get; set; }
        public ICollection<Likes>? Likes { get; set; }   
        public ICollection<VideoViews>? VideoViews { get; set; }    

    }
}
