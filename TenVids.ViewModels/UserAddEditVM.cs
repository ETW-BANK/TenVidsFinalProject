using System.ComponentModel.DataAnnotations;
using TenVids.ViewModels.CustomValidation;

namespace TenVids.ViewModels
{
   public class UserAddEditVM
    {
        public string Id { get; set; }
        [Display(Name="Name(Username)")]
        [StringCustomValidation("Name",true,3,15, "^[a-zA-Z\\s]+$", "Use letters only please")]
        public string Name { get; set; }

        [StringCustomValidation("Email", true, 0, 0, "^.+@[^\\.].*\\.[a-z]{2,}$", "Invalid email address")]
        public string Email { get; set; }

        [StringCustomValidation("Password", false, 6, 15, "^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,15}$", "Password must contain at least one letter, at least one number, and be between 6-15 characters in length with no special characters.")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }   
        public List<string> ApplicationRoles { get; set; }

    }
}
