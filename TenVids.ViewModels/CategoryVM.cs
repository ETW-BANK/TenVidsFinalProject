

using System.ComponentModel.DataAnnotations;

namespace TenVids.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
