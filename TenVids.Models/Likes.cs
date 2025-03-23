using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Models
{
   public class Likes
    {
    
        public bool IsLike { get; set; } 
        public string AppUserId { get; set; }
        public int VideoId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }
    }
}
