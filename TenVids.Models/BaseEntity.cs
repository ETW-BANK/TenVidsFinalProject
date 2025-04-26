using System.ComponentModel.DataAnnotations;

namespace TenVids.Models
{
   public class BaseEntity
    {
        [Key]
        public int Id { get; set; } 
    }
}
