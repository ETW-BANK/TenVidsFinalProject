using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TenVids.Models
{
   public class Videos:BaseEntity
    {
        

        [Required]
        public string? Thumbnail { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
      
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        public int ChannelId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("ChannelId")]
        public Channel Channel { get; set; }   
        public  VideoFiles? VideoFile { get; set; }
        public ICollection<Comment>? Comments { get; set; } 

        public ICollection<Likes>? Likes { get; set; }    

        public ICollection<VideoViews>? VideoViewers { get; set; }



    }
}
