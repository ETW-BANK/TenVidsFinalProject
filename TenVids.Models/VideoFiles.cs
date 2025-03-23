using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Models
{
  public class VideoFiles
    {
        [Key]
        public int Id { get; set; }
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
