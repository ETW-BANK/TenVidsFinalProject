
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
        [Required]
        public string ContentType { get; set; }
        [Required]
        public byte[] Contents { get; set; }
        public int CategoryId { get; set; }
        public int ChannelId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("ChannelId")]
        public Channel Channel { get; set; }   
        
        public ICollection<Comment>? Comments { get; set; } 

        public ICollection<Likes>? Likes { get; set; }    



    }
}
