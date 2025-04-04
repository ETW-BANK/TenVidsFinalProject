
using System.ComponentModel.DataAnnotations;

namespace TenVids.ViewModels
{
    public class ChannelAddEditVM
    {

        [Required]
        [Display(Name = "Channel Name")]
        [RegularExpression(@"^[a-zA-Z0-9]{3,15}", ErrorMessage = "Only Alphabets and Numbers are allowed")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Description of Your Channel")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "Description should be atleast {2} and Maximum of 200 characters")]
        public string Description { get; set; }

        public List<ErrorModelVM> Errors { get; set; } = new List<ErrorModelVM>();

    }
}
