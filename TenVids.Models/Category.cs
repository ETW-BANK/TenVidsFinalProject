using System.ComponentModel.DataAnnotations;
using Tensae.Generic.Repository;


namespace TenVids.Models
{
    public class Category:BaseEntity
    {
        [Required]
        public string? Name { get; set; }
        public ICollection<Videos>? Videos { get; set; } 
    }
}
