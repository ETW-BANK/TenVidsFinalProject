using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace TenVids.ViewModels
{
    public class VideoVM
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Upload Thumbnail here")]
        public IFormFile? ImageUpload { get; set; }
        public IFormFile? VideoUpload { get; set; }
        [Display(Name = "Choose Category")]
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
        public string? ImageContentTypes { get; set; } 
        public string? VideoContentTypes { get; set; }
        public string? ImageUrl { get; set; }   

    }
}
