using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Models
{
    public class VideoViews
    {
        public string AppUserId { get; set; } 
        public int VideoId { get; set; }   
        
        public ApplicationUser AppUser { get; set; }
        public Videos Video { get; set; }   

    }
}
