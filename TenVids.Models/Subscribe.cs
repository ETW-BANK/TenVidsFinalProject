using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Models
{
   public class Subscribe
    {
   
        public string AppUserId { get; set; } 
        public int ChannelId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public Channel Channel { get; set; }
    }
}
