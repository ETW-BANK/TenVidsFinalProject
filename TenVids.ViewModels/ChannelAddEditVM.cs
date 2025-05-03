using System.ComponentModel.DataAnnotations;

namespace TenVids.ViewModels
{
    public class ChannelAddEditVM
    {
        [Required]
        [Display(Name = "Channel name")]
        [RegularExpression("^[a-zA-Z]{3,15}", ErrorMessage = "Name must be between 3 and 15 characters long and can only contain letters (A-Z, a-z)")]
        public string Name { get; set; }
        [Required(ErrorMessage = "About field is required")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "About must be at least {2}, and maximum {1} characters")]
        [Display(Name = "About your channel")]
        public string About { get; set; }
        public List<ModelErrorVm> Errors { get; set; } = new List<ModelErrorVm>();
        public int SubscribersCount { get; set; }

    }
}
