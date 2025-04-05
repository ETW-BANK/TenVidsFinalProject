
using System.ComponentModel.DataAnnotations;


namespace TenVids.Models
{
    public class Category:BaseEntity
    {
       

        [Required]
        public string? Name { get; set; }

        public ICollection<Videos>? Videos { get; set; } 
    }
}
