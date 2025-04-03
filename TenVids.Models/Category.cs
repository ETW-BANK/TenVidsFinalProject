
using System.ComponentModel.DataAnnotations;


namespace TenVids.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public ICollection<Videos> Videos { get; set; } 
    }
}
