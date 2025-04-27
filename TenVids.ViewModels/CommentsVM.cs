
using System.ComponentModel.DataAnnotations;


namespace TenVids.ViewModels
{
   public class CommentsVM
    {
        public CommentPostVM PostComment{ get; set; }=new();
        public IEnumerable<AvailableCommentsVM>? AvailableComments { get; set; } 
    }
    public class CommentPostVM
    {
        [Required]
        public int VideoId { get; set; }
        [Required]
        public string Content { get; set; }
    }
    public class AvailableCommentsVM
    {
        public string Content { get; set; } 
        public string FormName { get; set; }    
        public int FormChannelId { get; set; }  
        public DateTime PostedAt { get; set; } 
    }
}
