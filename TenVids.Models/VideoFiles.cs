
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Tensae.Generic.Repository;

namespace TenVids.Models
{
  public class VideoFiles:BaseEntity
    {
        [Required]
        public string ContentType { get; set; }
        [Required]
        public byte[] Contents { get; set; }
        [Required]
        public string Extension { get; set; }

        public int VideoId { get; set; }
        // Navigation
        [ForeignKey("VideoId")]
        public Videos Video { get; set; }
    }
}
